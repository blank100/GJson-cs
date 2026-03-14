using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
	public partial class GJsonObject
	{
		private const NumberStyles LongNumberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
		private const NumberStyles DoubleNumberStyles = LongNumberStyles | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

		public static GJsonObject Decode(string jsonString) => Decode(jsonString.AsSpan());

		public static unsafe GJsonObject Decode(ReadOnlySpan<char> jsonChars) {
			GJsonObject root = null;
			fixed (char* start = jsonChars) {
				using RefStack<GJsonObject> stack = new();
				using RefStack<string> nameStack = new();
				var buffer = new RefWriter<char>(stackalloc char[256]);

				var current = start;
				var end = current + jsonChars.Length;
				try {
					while (current < end) {
						var c = *current++;
						if (char.IsWhiteSpace(c)) continue;
						//@formatter:off
						if (c == '{') { stack.Push(root = new(GJsonType.Object)); break; }
						if (c == '[') { stack.Push(root = new(GJsonType.Array)); break; }
						if (c == '/' && current < end && *current == '/') { DecodeComment(ref current, end); continue; }
						if (c == 'n' && end - current >= 3 && *current == 'u' && *(current + 1) == 'l' && *(current + 2) == 'l') return null;
						throw new FormatException("JSON 根必须是对象或数组");
						//@formatter:on
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
								continue;
							case '[':
								stack.Push(new(GJsonType.Array));
								continue;
							case ',' when v == null:
								continue;
							case ',':
								var top1 = stack.Peek();
								if (top1.Type == GJsonType.Object) {
									top1.Add(nameStack.Pop(), v);
									n = null;
								} else top1.Add(v);
								v = null;
								continue;
							case ':':
								if (n == null)
									throw new FormatException("属性名为空");
								nameStack.Push(n);
								continue;
							case '}':
							case ']':
								var top2 = stack.Pop();
								if (v != null) {
									if (top2.Type == GJsonType.Object) top2.Add(nameStack.Pop(), v);
									else top2.Add(v);
								}
								v = top2;
								continue;
						}

						if (c == '"') {
							var text = DecodeText(ref current, end, ref buffer);
							if (stack.Peek().Type == GJsonType.Object && n == null) n = text;
							else v = new(text);
							continue;
						}
						
						if (c == '-' || c == '+' || (uint)(c - '0') <= 9) v = DecodeNumber(ref current, end);
						else if (c == 't') v = DecodeTrue(ref current, end);
						else if (c == 'f') v = DecodeFalse(ref current, end);
						else if (c == 'n') v = DecodeNull(ref current, end);
						else if (c == '/' && current < end && *current == '/') DecodeComment(ref current, end);
						else throw new FormatException($"无法识别的字符 ‘{c}’");
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
			var startIndex = Math.Max(0, (int)(current - start) - 50);
			var length = Math.Min(100, (int)(end - start) - startIndex);
			return new(start + startIndex, 0, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe void DecodeComment(ref char* current, char* end) {
			current++;
			while (current < end && *current is not ('\n' or '\r')) current++;
		}

		private static unsafe string DecodeText(ref char* current, char* end, ref RefWriter<char> buffer) {
			var segmentStart = current;
			while (current < end) {
				var c = *current++;
				switch (c) {
					case '"':
						buffer.Write(new ReadOnlySpan<char>(segmentStart, (int)(current - segmentStart - 1)));
						goto AfterString;
					case '\\' when current < end: {
						buffer.Write(new ReadOnlySpan<char>(segmentStart, (int)(current - segmentStart - 1)));

						var esc = *current++;
						segmentStart = current;
						switch (esc) {
							//@formatter:off
							case '"':  buffer.Write('"');  break;
							case '\\': buffer.Write('\\'); break;
							case 'n':  buffer.Write('\n'); break;
							case 'r':  buffer.Write('\r'); break;
							case 't':  buffer.Write('\t'); break;
							case 'b':  buffer.Write('\b'); break;
							case 'f':  buffer.Write('\f'); break;
							//@formatter:on
							case 'u' when current + 4 <= end:
								var u1 = *current++;
								var u2 = *current++;
								var u3 = *current++;
								var u4 = *current++;
								buffer.Write(CharUtils.GetCodePoint(u1, u2, u3, u4));
								segmentStart = current;
								break;
							default: throw new FormatException($"无效的转义字符 '\\{esc}'");
						}
						break;
					}
				}
			}

			throw new FormatException("字符串缺少结尾引号");
		AfterString:
			var result = buffer.WrittenSpan.ToString();
			buffer.Clear();
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe GJsonObject DecodeNumber(ref char* current, char* end) {
			var s = current - 1;
			var isDouble = false;
			while (current < end) {
				var c = *current++;
				if (char.IsDigit(c) || c is '+' or '-') continue;
				if (c is '.' or 'e' or 'E') isDouble = true;
				else break;
			}

			var slice = new ReadOnlySpan<char>(s, (int)(--current - s));
			if (!isDouble)
				return long.TryParse(slice, LongNumberStyles, CultureInfo.InvariantCulture, out var t) ?
					new(t) :
					throw new($"Decode long fail: text:{new string(slice)}");
			else {
				if (!double.TryParse(slice, DoubleNumberStyles, CultureInfo.InvariantCulture, out var t)) throw new($"Decode double fail: text:{new string(slice)}");
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
