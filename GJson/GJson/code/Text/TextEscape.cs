using System;
using System.Runtime.CompilerServices;

namespace Gal.Core
{
    /// <summary>
    /// 文本转义
    /// </summary>
    /// <para>author gouanlin</para>
    public static class TextEscape
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string Exec(string text) {
            var forecastLength = (int)(text.Length * 1.1f);
            if (forecastLength <= 256) {
                RefWriter<char> writer = new(stackalloc char[forecastLength]);
                try {
                    Exec(text, ref writer);
                    return writer.writtenSpan.ToString();
                } finally {
                    writer.Dispose();
                }
            } else {
                RefWriter<char> writer = new(forecastLength);
                try {
                    Exec(text, ref writer);
                    return writer.writtenSpan.ToString();
                } finally {
                    writer.Dispose();
                }
            }
        }

        public static void Exec(ReadOnlySpan<char> text, ref RefWriter<char> writer) {
            start:

            var span = writer.span;
            var r = span.Length;
            int i = 0, j = 0, hint = 1;
            for (var l = text.Length; i < l; i++) {
                var c = text[i];
                if (c > 31) {
                    //c >= ' '
                    if (c is '"' or '\\') {
                        if ((r -= 2) > 0) {
                            span[j++] = '\\';
                            span[j++] = c;
                        } else {
                            hint = 2;
                            goto reset;
                        }
                    } else if (r-- > 0) span[j++] = c;
                    else goto reset;
                } else {
                    if (r-- <= 0) goto reset;
                    span[j++] = '\\';

                    if (c == '\t') {
                        if (r-- > 0) span[j++] = 't';
                        else goto reset;
                    } else if (c == '\n') {
                        if (r-- > 0) span[j++] = 'n';
                        else goto reset;
                    } else if (c == '\r') {
                        if (r-- > 0) span[j++] = 'r';
                        else goto reset;
                    } else if (c == '\b') {
                        if (r-- > 0) span[j++] = 'b';
                        else goto reset;
                    } else if (c == '\f') {
                        if (r-- > 0) span[j++] = 'f';
                        else goto reset;
                    } else {
                        if ((r -= 5) > 0) {
                            span[j++] = 'u';
                            var t = ((ushort)c).ToString("X4");
                            t.AsSpan().CopyTo(span);
                            j += t.Length;
                        } else {
                            hint = 5;
                            goto reset;
                        }
                    }
                }
            }

            writer.Advance(j);

            return;
            reset:
            text = text[i..];
            writer.Advance(j);
            writer.HintSize(Math.Max((int)(text.Length * 1.1f), text.Length + hint));
            goto start;
        }

        /// <summary>
        /// 此方法逻辑清晰,但性能要低一点
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exec1(ReadOnlySpan<char> text, ref RefWriter<char> writer) {
            foreach (var c in text) {
                if (c > 31) {//c >= ' '
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
    }
}