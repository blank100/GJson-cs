using System;
using System.Diagnostics;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    public partial class GJsonObject
    {
        public static GJsonObject Decode(string jsonString) => Decode(jsonString.AsSpan());

        public static unsafe GJsonObject Decode(ReadOnlySpan<char> jsonChars) {
            GJsonObject root = null;
            fixed (char* start = jsonChars) {
                 using Stack<GJsonObject> stack = new();
                using Stack<string> attrNameStack = new();
                var buffer = new RefWriter<char>(stackalloc char[256]);

                var current = start;
                var end = current + jsonChars.Length;
                try {
                    while (current < end) {
                        var c = *current++;
                        if (char.IsWhiteSpace(c)) continue;
                        if (c == '{') {
                            stack.Push(root = new(GJsonType.Object));
                            break;
                        }
                        if (c == '[') {
                            stack.Push(root = new(GJsonType.Array));
                            break;
                        }
                        switch (c) {
                            case '/' when current <= end && *current == '/':
                                DecodeComment(ref current, end);
                                break;
                            case 'n' when end - current >= 3 && *current++ == 'u' && *current++ == 'l' && *current++ == 'l': return null;
                            default: throw new("语法错误, json 的根必须为 object 或 array");
                        }
                    }
                    string n = null;
                    GJsonObject v = null;
                    while (current < end) {
                        var c = *current++;
                        if (char.IsWhiteSpace(c)) continue;
                        switch (c) {
                            case '{':
                                stack.Push(new(GJsonType.Object));
                                n = null;
                                break;
                            case '[':
                                stack.Push(new(GJsonType.Array));
                                break;
                            case '"': {
                                var t = DecodeText(ref current, end, ref buffer);
                                if (stack.Peek().type == GJsonType.Object && n == null) n = t;
                                else v = new(t);
                                break;
                            }
                            case ',' when v == null: continue;
                            case ',': {
                                var top = stack.Peek();
                                if (top.type == GJsonType.Object) {
                                    top.Add(attrNameStack.Pop(), v);
                                    n = null;
                                } else top.Add(v);
                                v = null;
                                break;
                            }
                            case ':' when n != null:
                                attrNameStack.Push(n);
                                break;
                            case ':': throw new("属性名为空");
                            case '}': {
                                var top = stack.Pop();
                                if (v != null) top.Add(attrNameStack.Pop(), v);
                                v = top;
                                break;
                            }
                            case ']': {
                                var top = stack.Pop();
                                if (v != null) top.Add(v);
                                v = top;
                                break;
                            }
                            default: {
                                if (char.IsDigit(c) || c is '-' or '+') v = DecodeNumber(ref current, end);
                                else
                                    switch (c) {
                                        case 't':
                                            v = DecodeTrue(ref current, end);
                                            break;
                                        case 'f':
                                            v = DecodeFalse(ref current, end);
                                            break;
                                        case 'n':
                                            v = DecodeNull(ref current, end);
                                            break;
                                        case '/' when current <= end && *current == '/':
                                            DecodeComment(ref current, end);
                                            break;
                                        default: throw new($"语法错误");
                                    }

                                break;
                            }
                        }
                    }
                } catch (Exception e) {
                    throw new($"{e}, \nat:{GetErrorBlock(start, current, end)}");
                } finally {
                    buffer.Dispose();
                }
            }
            return root;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe string GetErrorBlock(char* start, char* current, char* end) {
            var s = (int)Math.Max(0, current - start - 50);
            return new(start, s, Math.Min(100, (int)(end - s)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void DecodeComment(ref char* current, char* end) {
            while (++current < end && *current++ is not ('\n' or '\r')) { }
        }

        private static unsafe string DecodeText(ref char* current, char* end, ref RefWriter<char> buffer) {
            var s = current;
            while (current < end) {
                var c = *current++;
                if (c == '"') {
                    buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                    break;
                }

                if (c != '\\') continue;
                var n = *current;
                switch (n) {
                    case '"':
                    case '\\':
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        s = ++current;
                        buffer.Write(n);
                        break;
                    case 't':
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        s = ++current;
                        buffer.Write('\t');
                        break;
                    case 'n':
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        s = ++current;
                        buffer.Write('\n');
                        break;
                    case 'r':
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        s = ++current;
                        buffer.Write('\r');
                        break;
                    case 'b':
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        s = ++current;
                        buffer.Write('\b');
                        break;
                    case 'f':
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        s = ++current;
                        buffer.Write('\f');
                        break;
                    case 'u' when current + 4 < end:
                        buffer.Write(new ReadOnlySpan<char>(s, (int)(current - s) - 1));
                        buffer.Write(CharUtils.GetCodePoint(*++current, *++current, *++current, *++current));
                        s = ++current;
                        break;
                }
            }

            var result = buffer.writtenSpan.ToString();
            buffer.Clear();

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe GJsonObject DecodeNumber(ref char* current, char* end) {
            var s = current - 1;
            var isDouble = false;
            while (current <= end) {
                var c = *current++;
                if (char.IsDigit(c) || c is '+' or '-' or 'e' or 'E') continue;
                if (c is not '.') break;
                isDouble = true;
            }

            var slice = new ReadOnlySpan<char>(s, (int)(--current - s));
            if (!isDouble) return new(long.Parse(slice));
            var result = double.Parse(slice);
            var longValue = (long)result;
            return Number.Equals(result, longValue) ? new(longValue) : new(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe GJsonObject DecodeTrue(ref char* current, char* end) {
            if (end - current < 3 || *current++ != 'r' || *current++ != 'u' || *current++ != 'e') throw new("语法错误");
            return new(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe GJsonObject DecodeFalse(ref char* current, char* end) {
            if (end - current < 4 || *current++ != 'a' || *current++ != 'l' || *current++ != 's' || *current++ != 'e') throw new("语法错误");
            return new(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe GJsonObject DecodeNull(ref char* current, char* end) {
            if (end - current < 3 || *current++ != 'u' || *current++ != 'l' || *current++ != 'l') throw new("语法错误");
            return new(GJsonType.Null);
        }

        private class Stack<T> : IDisposable
        {
            private const int DEFAULT_CAPACITY = 8;

            private T[] m_Array;
            private int m_Depth;

            public Stack(int capacity = DEFAULT_CAPACITY) {
                Debug.Assert(capacity > 0, $"Parameter 'capacity' cannot be negative, capacity:{capacity}");
                m_Array = ArrayPool<T>.Shared.Rent(capacity);
                m_Depth = 0;
            }

            public void Clear() {
                Array.Clear(m_Array, 0, m_Depth);
                m_Depth = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Peek(int i = 0) {
                Debug.Assert(m_Depth > 0, "Is empty");
                Debug.Assert(i >= 0, $"Parameter 'i' cannot be negative, i:{i}");
                Debug.Assert(i < m_Depth, "Parameter 'I' cannot be greater than or equal to the depth of the stack");
                return m_Array[m_Depth - 1 - i];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Push(T item) {
                if (m_Depth == m_Array.Length) {
                    var newArray = ArrayPool<T>.Shared.Rent(Math.Max(DEFAULT_CAPACITY, 2 * m_Array.Length));
                    Buffer.BlockCopy(m_Array, 0, newArray, 0, m_Depth);
                    m_Array = newArray;
                }

                m_Array[m_Depth++] = item;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Pop() {
                Debug.Assert(m_Depth > 0, "Is empty");
                var item = m_Array[--m_Depth];
                m_Array[m_Depth] = default;
                return item;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() {
                ArrayPool<T>.Shared.Return(m_Array);
                m_Array = null;
            }
        }
    }
}