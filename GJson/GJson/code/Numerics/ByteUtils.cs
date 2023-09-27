using System.Runtime.CompilerServices;

namespace Gal.Core {
	/// <summary>
	/// 字节操作
	/// </summary>
	/// <para>author gouanlin</para>
	public static class ByteUtils {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWhiteSpace(byte v) {
			// U+0009 = <control> HORIZONTAL TAB	\t
			// U+000a = <control> LINE FEED			\n
			// U+000b = <control> VERTICAL TAB
			// U+000c = <control> FORM FEED
			// U+000d = <control> CARRIAGE RETURN	\r
			// U+0085 = <control> NEXT LINE
			// U+00a0 = NO-BREAK SPACE
			return v == ' ' || v >= '\x0009' && v <= '\x000d' || v == '\x00a0' || v == '\x0085';
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(byte v) => v is >= (byte) '0' and <= (byte) '9';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLetter(byte v) => v >= 'a' && v <= 'z' || v >= 'A' && v <= 'Z';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ToUpper(byte v) => (byte) (v & ~0x20);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToNumber(byte x) {
			if ('0' <= x && x <= '9') {
				return x - '0';
			}
			if ('a' <= x && x <= 'f') {
				return x - 'a' + 10;
			}
			if ('A' <= x && x <= 'F') {
				return x - 'A' + 10;
			}
			throw new("Invalid Character" + x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetCodePoint(byte a, byte b, byte c, byte d) => ((ToNumber(a) * 16 + ToNumber(b)) * 16 + ToNumber(c)) * 16 + ToNumber(d);
	}
}