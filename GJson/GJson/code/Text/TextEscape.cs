using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gal.Core
{
	/// <summary>
	/// 文本转义
	/// </summary>
	/// <author>gouanlin</author>
	public static class TextEscape
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string Exec(string text) {
			var n = text.Length;
			var cap = (int)(n * 1.25f);
			var buf = cap <= 256 ? stackalloc char[cap] : new char[cap];
			var writer = new RefWriter<char>(buf);
			try {
				Exec(text, ref writer);
				return writer.WrittenSpan.ToString();
			} finally {
				writer.Dispose();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string Unescape(string text) {
			var n = text.Length;
			var cap = (int)(n * 1.25f);
			var buf = cap <= 256 ? stackalloc char[cap] : new char[cap];
			var writer = new RefWriter<char>(buf);
			try {
				Unescape(text, ref writer);
				return writer.WrittenSpan.ToString();
			} finally {
				writer.Dispose();
			}
		}

		public static void Exec(ReadOnlySpan<char> text, ref RefWriter<char> writer) {
			int i = 0, start = 0, remaining = text.Length;

			var span = writer.Span;
			int j = 0, capacity = span.Length;

			ref var src = ref MemoryMarshal.GetReference(text);
			ref var des = ref MemoryMarshal.GetReference(span);
			while (i < remaining) {
				var c = Unsafe.Add(ref src, i);
				if (c >= ' ' && c != '"' && c != '\\') {
					i++;
					continue;
				}

				if (start < i) {
					var batchLen = i - start;
					if (writer.InternalEnsureCapacity(ref capacity, batchLen + 2, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
					text.Slice(start, batchLen).CopyTo(span[j..]);
					j += batchLen;
				} else if (writer.InternalEnsureCapacity(ref capacity, 2, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
				Unsafe.Add(ref des, j++) = '\\';
				if (c >= ' ') Unsafe.Add(ref des, j++) = c;
				else {
					switch (c) {
						//@formatter:off
						case '\t': Unsafe.Add(ref des, j++) = 't'; break;
						case '\n': Unsafe.Add(ref des, j++) = 'n'; break;
						case '\r': Unsafe.Add(ref des, j++) = 'r'; break;
						case '\b': Unsafe.Add(ref des, j++) = 'b'; break;
						case '\f': Unsafe.Add(ref des, j++) = 'f'; break;
						//@formatter:on
						default:
							Unsafe.Add(ref des, j++) = 'u';
							if (writer.InternalEnsureCapacity(ref capacity, 4, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
							ushort code = c;
							Unsafe.Add(ref des, j + 0) = GetHex(code >> 12 & 0xF);
							Unsafe.Add(ref des, j + 1) = GetHex(code >> 8 & 0xF);
							Unsafe.Add(ref des, j + 2) = GetHex(code >> 4 & 0xF);
							Unsafe.Add(ref des, j + 3) = GetHex(code & 0xF);
							j += 4;
							break;
					}
				}

				start = ++i;
			}

			if (start < remaining) {
				var batchLen = remaining - start;
				if (writer.InternalEnsureCapacity(ref capacity, batchLen, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
				text.Slice(start, batchLen).CopyTo(span[j..]);
				j += batchLen;
			}

			writer.Advance(j);
		}

		/// <summary>
		/// 此方法逻辑清晰,但性能要低一点
		/// </summary>
		/// <param name="text"></param>
		/// <param name="writer"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Exec1(ReadOnlySpan<char> text, ref RefWriter<char> writer) {
			foreach (var c in text) {
				if (c > 31) {
					//c >= ' '
					if (c is '"' or '\\') writer.Write('\\', c);
					else writer.Write(c);
				} else {
					switch (c) {
						case '\t':
							writer.Write('\\', 't');
							break;
						case '\n':
							writer.Write('\\', 'n');
							break;
						case '\r':
							writer.Write('\\', 'r');
							break;
						case '\b':
							writer.Write('\\', 'b');
							break;
						case '\f':
							writer.Write('\\', 'f');
							break;
						default:
							writer.Write('\\', 'u');
							writer.Write(((ushort)c).ToString("X4"));
							break;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Unescape(ReadOnlySpan<char> text, ref RefWriter<char> writer) {
			int i = 0, start = 0, remaining = text.Length;

			var span = writer.Span;
			int j = 0, capacity = span.Length;

			ref var src = ref MemoryMarshal.GetReference(text);
			ref var des = ref MemoryMarshal.GetReference(span);

			while (i < remaining) {
				var c = Unsafe.Add(ref src, i);
				if (c == '\\') {
					if (i > start) {
						var batchLen = i - start;
						if (writer.InternalEnsureCapacity(ref capacity, batchLen, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
						text.Slice(start, batchLen).CopyTo(span[j..]);
						j += batchLen;
					}

					if (++i >= remaining) break;
					c = Unsafe.Add(ref src, i++);
					switch (c) {
						// 基本转义字符
						case '"':
						case '\\':
						//@formatter:off
						case '/': if (writer.InternalEnsureCapacity(ref capacity,1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span); Unsafe.Add(ref des, j++) = c; break;
						case 'b': if (writer.InternalEnsureCapacity(ref capacity,1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span); Unsafe.Add(ref des, j++) = '\b'; break;
						case 'f': if (writer.InternalEnsureCapacity(ref capacity,1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span); Unsafe.Add(ref des, j++) = '\f'; break;
						case 'n': if (writer.InternalEnsureCapacity(ref capacity,1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span); Unsafe.Add(ref des, j++) = '\n'; break;
						case 'r': if (writer.InternalEnsureCapacity(ref capacity,1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span); Unsafe.Add(ref des, j++) = '\r'; break;
						case 't': if (writer.InternalEnsureCapacity(ref capacity,1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span); Unsafe.Add(ref des, j++) = '\t'; break;
						//@formatter:on

						// Unicode 转义
						case 'u':
							if (i + 4 > remaining) throw new FormatException("Invalid \\uXXXX sequence");
							var code = (ushort)(HexVal(Unsafe.Add(ref src, i)) << 12 |
							                    HexVal(Unsafe.Add(ref src, i + 1)) << 8 |
							                    HexVal(Unsafe.Add(ref src, i + 2)) << 4 |
							                    HexVal(Unsafe.Add(ref src, i + 3)));
							if (writer.InternalEnsureCapacity(ref capacity, 1, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
							Unsafe.Add(ref des, j++) = (char)code;
							i += 4;
							break;

						default:
							if (writer.InternalEnsureCapacity(ref capacity, 2, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
							Unsafe.Add(ref des, j++) = '\\';
							Unsafe.Add(ref des, j++) = c;
							break;
					}
					start = i;
				} else i++;
			}

			if (start < remaining) {
				var batchLen = remaining - start;
				if (writer.InternalEnsureCapacity(ref capacity, batchLen, ref j, remaining, ref span)) des = ref MemoryMarshal.GetReference(span);
				text.Slice(start, batchLen).CopyTo(span[j..]);
				j += batchLen;
			}

			writer.Advance(j);
		}

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// private static bool EnsureCapacity(ref int capacity, int need, ref int written, int remaining, ref RefWriter<char> writer, ref Span<char> span) {
		// 	if (capacity - written >= need) return false;
		// 	writer.Advance(written);
		// 	written = 0;
		// 	writer.HintSize(Math.Max((int)(remaining * 1.1f), remaining + need));
		// 	span = writer.span;
		// 	capacity = span.Length;
		// 	return true;
		// }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static char GetHex(int val) => (char)(val < 10 ? '0' + val : 'A' + (val - 10));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int HexVal(char c) => c <= '9' ? c - '0' : (c & ~32) - 'A' + 10;
	}
}
