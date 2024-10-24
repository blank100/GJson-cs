using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    public partial class GJsonObject
    {
        private const NumberStyles LONG_NUMBER_STYLES = NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
        private const NumberStyles DOUBLE_NUMBER_STYLES = LONG_NUMBER_STYLES | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        public static GJsonObject Decode(string jsonString) => Decode(jsonString.AsSpan());

        public static unsafe GJsonObject Decode(ReadOnlySpan<char> jsonChars) {
            GJsonObject root = null;
            fixed (char* start = jsonChars) {
                using RefStack<GJsonObject> stack = new();
                using RefStack<string> attrNameStack = new();
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
        private static unsafe string GetErrorBlock(char* start, char* current, char* end) => new(start, (int)Math.Max(0, current - start - 50), Math.Min(100, (int)(end - start)));

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
                if (char.IsDigit(c) || c is '+' or '-') continue;
                if (c is '.' or 'e' or 'E') isDouble = true;
                else break;
            }

            var slice = new ReadOnlySpan<char>(s, (int)(--current - s));
            if (!isDouble)
                return long.TryParse(slice, LONG_NUMBER_STYLES, CultureInfo.InvariantCulture, out var t) ?
                    new(t) :
                    throw new($"Decode long fail: text:{new string(slice)}");
            else {
                if (!double.TryParse(slice, DOUBLE_NUMBER_STYLES, CultureInfo.InvariantCulture, out var t)) throw new($"Decode double fail: text:{new string(slice)}");
                var l = (long)t;
                return Number.Equals(t, l) ? new(l) : new(t);
            }
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
    }
}