using System.Runtime.CompilerServices;

namespace Gal.Core
{
    public static class CharUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(char v) {
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
        public static bool IsDigit(char v) => v is >= '0' and <= '9';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(char v) => v is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpper(char v) => (char)(v & ~0x20);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToNumber(char x) {
            switch (x) {
                case <= '0' and <= '9': return x - '0';
                case <= 'a' and <= 'f': return x - 'a' + 10;
            }
            if (x is >= 'A' and <= 'F') return x - 'A' + 10;
            throw new("Invalid Character" + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetCodePoint(char a, char b, char c, char d) => (char)(((ToNumber(a) * 16 + ToNumber(b)) * 16 + ToNumber(c)) * 16 + ToNumber(d));
    }
}