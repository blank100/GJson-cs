using System;
using System.Collections.Generic;

namespace Gal.Core.GJson
{
	/// <summary>
	/// json解析器
	/// </summary>
	/// <para>author gouanlin</para>
	public static partial class GJsonDecoder
	{
		private static readonly SafePool<Stack<GJsonObject>> m_StackPool = new(() => new(8), t => t.Clear(), null, 4);
		private static readonly SafePool<Stack<string>> m_AttrNameStackPool = new(() => new(8), t => t.Clear(), null, 4);

		public static GJsonObject Exec(string jsonString) {
			var text = jsonString.AsSpan();

			GJsonObject root = null;

			var buffer = new RefWriter<char>(stackalloc char[256]);

			var stack = m_StackPool.Get();
			var attrNameStack = m_AttrNameStackPool.Get();
			try {
				int i = 0, l = text.Length;
				while (i < l) {
					var c = text[i++];
					if (char.IsWhiteSpace(c)) continue;
					if (c == '{') {
						stack.Push(root = GJsonObject.Get(GJsonType.Object));
						break;
					}
					if (c == '[') {
						stack.Push(root = GJsonObject.Get(GJsonType.Array));
						break;
					}
					if (c == '/') {
						if (i < l && text[i] == '/') {
							var t = text[i..].IndexOfAny('\n', '\r');
							i = t > 0 ? i + t + 1 : l;
							continue;
						}
					} else if (c == 'n') {
						if (l - i >= 3 && text[i] == 'u' && text[i + 1] == 'l' && text[i + 2] == 'l') return null;
					}
					throw new($"语法错误, json 的根必须为 object 或 array, at:{GetErrorBlock(text, i, l)}");
				}

				GJsonObject v = null;
				while (i < l) {
					var c = text[i++];
					if (char.IsWhiteSpace(c)) continue;
					switch (c) {
						case '{':
							stack.Push(GJsonObject.Get(GJsonType.Object));
							break;
						case '[':
							stack.Push(GJsonObject.Get(GJsonType.Array));
							break;
						case '"':
							v = DecodeString(text, ref i, l, ref buffer);
							break;
						case ',' when v == null: continue;
						case ',': {
								var top = stack.Peek();
								if (top.type == GJsonType.Object) {
									top.Add(attrNameStack.Pop(), v);
								} else {
									top.Add(v);
								}
								v = null;
								break;
							}
						case ':' when v != null:
							attrNameStack.Push(v);
							v.Dispose();
							v = null;
							break;
						case ':': throw new($"属性名为空, at:{GetErrorBlock(text, i, l)}");
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
								if (char.IsDigit(c) || c is '-' or '+') {
									buffer.Write(c);
									v = DecodeNumber(text, ref i, l, ref buffer);
								} else
									switch (c) {
										case 't':
										case 'f': {
												buffer.Write(c);
												var t = DecodeKeyword(text, ref i, l, ref buffer);
												v = t switch {
													"true" => true,
													"false" => false,
													_ => throw new($"不能识别的关键字{t}, at:{GetErrorBlock(text, i, l)}")
												};
												break;
											}
										case 'n': {
												buffer.Write(c);
												var t = DecodeKeyword(text, ref i, l, ref buffer);
												v = t switch {
													"null" => GJsonObject.Get(GJsonType.Null),
													_ => throw new($"不能识别的关键字{t}, at:{GetErrorBlock(text, i, l)}")
												};
												break;
											}
										case '/' when i < l && text[i] == '/': {
												var t = text[i..].IndexOfAny('\n', '\r');
												i = t > 0 ? i + t + 1 : l;
												break;
											}
										default: throw new($"语法错误, at:{GetErrorBlock(text, i, l)}");
									}
								break;
							}
					}
				}
			} finally {
				buffer.Dispose();

				stack.Clear();
				attrNameStack.Clear();
			}

			return root;
		}

		private static string GetErrorBlock(ReadOnlySpan<char> text, int i, int l) {
			var start = Math.Max(0, i - 50);
			var length = Math.Min(100, l - start);
			return text.Slice(start, length).ToString();
		}

		private static string DecodeString(ReadOnlySpan<char> text, ref int i, int l, ref RefWriter<char> buffer) {
			while (i < l) {
				var c = text[i];
				if (c == '"') {
					i++;
					break;
				}
				if (c == '\\' && i + 1 < l) {
					var n = text[i + 1];
					switch (n) {
						case '"':
						case '\\':
							buffer.Write(n);
							i += 2;
							break;
						case 't':
							buffer.Write('\t');
							i += 2;
							break;
						case 'n':
							buffer.Write('\n');
							i += 2;
							break;
						case 'r':
							buffer.Write('\r');
							i += 2;
							break;
						case 'b':
							buffer.Write('\b');
							i += 2;
							break;
						case 'f':
							buffer.Write('\f');
							i += 2;
							break;
						case 'u' when i + 5 < l:
							buffer.Write(CharUtils.GetCodePoint(text[i + 2], text[i + 3], text[i + 4], text[i + 5]));
							i += 6;
							break;
						default:
							buffer.Write(c);
							i++;
							break;
					}
				} else {
					buffer.Write(c);
					i++;
				}
			}

			var result = buffer.writtenSpan.ToString();
			buffer.Clear();

			return result;
		}

		private static GJsonObject DecodeNumber(ReadOnlySpan<char> text, ref int i, int l, ref RefWriter<char> buffer) {
			var isDouble = false;
			while (i < l) {
				var c = text[i];
				if (char.IsDigit(c) || c == '+' || c == '-' || char.IsWhiteSpace(c)) {
					buffer.Write(c);
					i++;
				} else if (c == '.' || c == 'e' || c == 'E') {
					isDouble = true;
					buffer.Write(c);
					i++;
				} else {
					break;
				}
			}

			var result = isDouble ? double.Parse(buffer.writtenSpan) : long.Parse(buffer.writtenSpan);
			buffer.Clear();

			return result;
		}

		private static string DecodeKeyword(ReadOnlySpan<char> text, ref int i, int l, ref RefWriter<char> buffer) {
			while (i < l) {
				var c = text[i];
				if (char.IsLetter(c) || char.IsWhiteSpace(c)) {
					buffer.Write(c);
					i++;
				} else {
					break;
				}
			}

			var result = buffer.writtenSpan.ToString().Trim();
			buffer.Clear();

			return result;
		}
	}
}