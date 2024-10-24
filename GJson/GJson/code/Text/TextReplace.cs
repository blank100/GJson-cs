using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gal.Core
{
    /// <summary>
    /// </summary>
    /// <para>author gouanlin</para>
    public static class TextReplace
    {
        private static readonly string[] s_Arguments1 = new string[1];
        private static readonly string[] s_Arguments2 = new string[2];
        private static readonly string[] s_Arguments3 = new string[3];
        private static readonly string[] s_Arguments4 = new string[4];

        /// <summary>
        /// 非多线程安全
        /// </summary>
        /// <param name="text"></param>
        /// <param name="argument0"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Replace(ReadOnlySpan<char> text, string argument0) {
            s_Arguments1[0] = argument0;
            return Replace(text, s_Arguments1);
        }

        /// <summary>
        /// 非多线程安全
        /// </summary>
        /// <param name="text"></param>
        /// <param name="argument0"></param>
        /// <param name="argument1"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Replace(ReadOnlySpan<char> text, string argument0, string argument1) {
            s_Arguments2[0] = argument0;
            s_Arguments2[1] = argument1;
            return Replace(text, s_Arguments2);
        }

        /// <summary>
        /// 非多线程安全
        /// </summary>
        /// <param name="text"></param>
        /// <param name="argument0"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Replace(ReadOnlySpan<char> text, string argument0, string argument1, string argument2) {
            s_Arguments3[0] = argument0;
            s_Arguments3[1] = argument1;
            s_Arguments3[2] = argument2;
            return Replace(text, s_Arguments3);
        }

        /// <summary>
        /// 非多线程安全
        /// </summary>
        /// <param name="text"></param>
        /// <param name="argument0"></param>
        /// <param name="argument1"></param>
        /// <param name="argument2"></param>
        /// <param name="argument3"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Replace(ReadOnlySpan<char> text, string argument0, string argument1, string argument2, string argument3) {
            s_Arguments4[0] = argument0;
            s_Arguments4[1] = argument1;
            s_Arguments4[2] = argument2;
            s_Arguments4[3] = argument3;
            return Replace(text, s_Arguments4);
        }

        /// <summary>
        /// 同 string.Format
        /// </summary>
        /// <param name="text"></param>
        /// <param name="arguments"></param>
        /// <param name="prefixDelimiter"></param>
        /// <param name="suffixDelimiter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string Replace(ReadOnlySpan<char> text, ReadOnlySpan<string> arguments, char prefixDelimiter = '{', char suffixDelimiter = '}') {
            var forecastLength = (int)(text.Length * 1.25f);
            if (forecastLength <= 256) {
                RefWriter<char> buffer = new(stackalloc char[forecastLength]);
                try {
                    Replace(ref buffer, text, arguments, prefixDelimiter, suffixDelimiter);
                    return buffer.writtenSpan.ToString();
                } finally {
                    buffer.Dispose();
                }
            } else {
                RefWriter<char> buffer = new(forecastLength);
                try {
                    Replace(ref buffer, text, arguments, prefixDelimiter, suffixDelimiter);
                    return buffer.writtenSpan.ToString();
                } finally {
                    buffer.Dispose();
                }
            }
        }

        public static void Replace(ref RefWriter<char> buffer, ReadOnlySpan<char> text, ReadOnlySpan<string> arguments, char prefixDelimiter = '{', char suffixDelimiter = '}') {
            var argCount = arguments.Length;
            int i;
            while ((i = text.IndexOf(prefixDelimiter)) != -1) {
                buffer.Write(text[..i++]);
                text = text[i..];
                if ((i = text.IndexOfAny(suffixDelimiter, prefixDelimiter)) != -1) {
                    if (text[i] == prefixDelimiter) {
                        buffer.Write(prefixDelimiter);
                        continue;
                    }
                    var key = text[..i++];
                    if (int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) && argCount > number) buffer.Write(arguments[number]);
                    else {
                        buffer.Write(prefixDelimiter);
                        buffer.Write(key);
                        buffer.Write(suffixDelimiter);
                    }

                    text = text[i..];
                } else buffer.Write(prefixDelimiter);
            }
            buffer.Write(text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string Replace(ReadOnlySpan<char> text, IDictionary<string, string> replaceQueryTable, char prefixDelimiter = '{', char suffixDelimiter = '}') {
            var forecastLength = (int)(text.Length * 1.25f);
            if (forecastLength <= 256) {
                RefWriter<char> buffer = new(stackalloc char[forecastLength]);
                try {
                    Replace(ref buffer, text, replaceQueryTable, prefixDelimiter, suffixDelimiter);
                    return buffer.writtenSpan.ToString();
                } finally {
                    buffer.Dispose();
                }
            } else {
                RefWriter<char> buffer = new(forecastLength);
                try {
                    Replace(ref buffer, text, replaceQueryTable, prefixDelimiter, suffixDelimiter);
                    return buffer.writtenSpan.ToString();
                } finally {
                    buffer.Dispose();
                }
            }
        }

        public static void Replace(
            ref RefWriter<char> buffer
            , ReadOnlySpan<char> text
            , IDictionary<string, string> replaceQueryTable
            , char prefixDelimiter = '{'
            , char suffixDelimiter = '}'
        ) {
            int i;
            while ((i = text.IndexOf(prefixDelimiter)) != -1) {
                buffer.Write(text[..i++]);
                text = text[i..];
                if ((i = text.IndexOfAny(suffixDelimiter, prefixDelimiter)) != -1) {
                    if (text[i] == prefixDelimiter) {
                        buffer.Write(prefixDelimiter);
                        continue;
                    }
                    var key = text[..i++];
                    if (replaceQueryTable.TryGetValue(key.ToString(), out var content)) buffer.Write(content);
                    else {
                        buffer.Write(prefixDelimiter);
                        buffer.Write(key);
                        buffer.Write(suffixDelimiter);
                    }

                    text = text[i..];
                } else buffer.Write(prefixDelimiter);
            }
            buffer.Write(text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string Replace(
            ReadOnlySpan<char> text
            , IDictionary<string, string> replaceQueryTable
            , ReadOnlySpan<char> prefixDelimiter
            , ReadOnlySpan<char> suffixDelimiter
        ) {
            var forecastLength = (int)(text.Length * 1.25f);
            if (forecastLength <= 256) {
                RefWriter<char> buffer = new(stackalloc char[forecastLength]);
                try {
                    Replace(ref buffer, text, replaceQueryTable, prefixDelimiter, suffixDelimiter);
                    return buffer.writtenSpan.ToString();
                } finally {
                    buffer.Dispose();
                }
            } else {
                RefWriter<char> buffer = new(forecastLength);
                try {
                    Replace(ref buffer, text, replaceQueryTable, prefixDelimiter, suffixDelimiter);
                    return buffer.writtenSpan.ToString();
                } finally {
                    buffer.Dispose();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Replace(
            ref RefWriter<char> buffer
            , ReadOnlySpan<char> text
            , IDictionary<string, string> replaceQueryTable
            , ReadOnlySpan<char> prefixDelimiter
            , ReadOnlySpan<char> suffixDelimiter
        ) {
            if (prefixDelimiter.Length == 1 && suffixDelimiter.Length == 1) Replace(ref buffer, text, replaceQueryTable, prefixDelimiter[0], suffixDelimiter[0]);
            else {
                fixed (char* pText = text) {
                    Replace(pText, pText + text.Length, ref buffer, replaceQueryTable, prefixDelimiter, suffixDelimiter);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Replace(
            char* start
            , char* end
            , ref RefWriter<char> buffer
            , IDictionary<string, string> replaceQueryTable
            , ReadOnlySpan<char> prefixDelimiter
            , ReadOnlySpan<char> suffixDelimiter
        ) {
            var pFirst = prefixDelimiter[0];
            prefixDelimiter = prefixDelimiter[1..];
            var sFirst = suffixDelimiter[0];
            suffixDelimiter = suffixDelimiter[1..];

            var current = start;
            char* keyStart = null;
            while (current < end) {
                var c = *current++;
                if (c == pFirst && Match(current, end, prefixDelimiter)) {
                    current = keyStart = current + prefixDelimiter.Length;
                } else if (keyStart != null && c == sFirst && Match(current, end, suffixDelimiter)) {
                    var key = new string(keyStart, 0, (int)(current - 1 - keyStart));
                    if (replaceQueryTable.TryGetValue(key, out var content)) {
                        buffer.Write(new ReadOnlySpan<char>(start, (int)(keyStart - prefixDelimiter.Length - 1 - start)));
                        buffer.Write(content);
                        start = current + suffixDelimiter.Length;
                        current = start;
                    }
                    keyStart = null;
                }
            }
            buffer.Write(new ReadOnlySpan<char>(start, (int)(end - start)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool Match(char* begin, char* end, ReadOnlySpan<char> value) {
            var l = value.Length;
            if ((int)(end - begin) < l) return false;
            for (var i = 0; i < l; i++) {
                if (begin[i] != value[i]) return false;
            }
            return true;
        }
    }
}