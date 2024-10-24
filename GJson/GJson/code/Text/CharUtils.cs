using System.Runtime.CompilerServices;

namespace Gal.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <para>author gouanlin</para>
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
            return v is ' ' or >= '\x0009' and <= '\x000d' or '\x00a0' or '\x0085';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(char v) => v is >= '0' and <= '9';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(char v) => v is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpper(char v) => (char)(v & ~0x20);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToNumber(char x) {
            return x switch {
                >= '0' and <= '9' => x - '0'
                , >= 'a' and <= 'f' => x - 'a' + 10
                , >= 'A' and <= 'F' => x - 'A' + 10
                , _ => throw new($"Invalid Character {x}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetCodePoint(char a, char b, char c, char d) => (char)(((ToNumber(a) * 16 + ToNumber(b)) * 16 + ToNumber(c)) * 16 + ToNumber(d));
    }
}