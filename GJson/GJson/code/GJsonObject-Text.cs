using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    /// <summary>
    /// json对象
    /// </summary>
    /// <para>author gouanlin</para>
    public partial class GJsonObject
    {
        private static readonly ReadOnlyMemory<char> s_Null = "null".AsMemory();
        private static readonly ReadOnlyMemory<char> s_True = "true".AsMemory();
        private static readonly ReadOnlyMemory<char> s_False = "false".AsMemory();
        private static readonly ReadOnlyMemory<char> s_Undefined = "undefined".AsMemory();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() {
            RefWriter<char> buffer = new(stackalloc char[256]);
            try {
                BuildJsonString(ref buffer, null, CultureInfo.InvariantCulture);
                return buffer.writtenSpan.ToString();
            } finally {
                buffer.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(bool format, string numberFormat = null, IFormatProvider formatProvider = null) {
            RefWriter<char> buffer = new(stackalloc char[256]);
            try {
                if (format) BuildJsonStringWithFormat(ref buffer, numberFormat, formatProvider ?? CultureInfo.InvariantCulture);
                else BuildJsonString(ref buffer, numberFormat, formatProvider ?? CultureInfo.InvariantCulture);
                return buffer.writtenSpan.ToString();
            } finally {
                buffer.Dispose();
            }
        }

        private void BuildJsonString(ref RefWriter<char> buffer, string numberFormat, IFormatProvider formatProvider) {
            switch (type) {
                case GJsonType.String:
                    if (m_String == null) buffer.Write(s_Null);
                    else {
                        buffer.Write('"');
                        TextEscape.Exec(m_String, ref buffer);
                        buffer.Write('"');
                    }
                    break;
                case GJsonType.Long:
                    buffer.Write(m_Long.ToString(numberFormat, formatProvider));
                    break;
                case GJsonType.Double:
                    buffer.Write(m_Double.ToString(numberFormat, formatProvider));
                    break;
                case GJsonType.Object when m_Dict.Count == 0:
                    buffer.Write('{', '}');
                    break;
                case GJsonType.Object: {
                    buffer.Write('{');
                    foreach (var (key, value) in m_Dict) {
                        buffer.Write('"');
                        TextEscape.Exec(key, ref buffer);
                        buffer.Write('"', ':');
                        value.BuildJsonString(ref buffer, numberFormat, formatProvider);
                        buffer.Write(',');
                    }

                    buffer.length--;
                    buffer.Write('}');
                    break;
                }
                case GJsonType.Array when m_List.Count == 0:
                    buffer.Write('[', ']');
                    break;
                case GJsonType.Array: {
                    buffer.Write('[');
                    foreach (var item in m_List) {
                        item.BuildJsonString(ref buffer, numberFormat, formatProvider);
                        buffer.Write(',');
                    }

                    buffer.length--;
                    buffer.Write(']');
                    break;
                }
                case GJsonType.Boolean:
                    buffer.Write(m_Long != 0 ? s_True : s_False);
                    break;
                case GJsonType.Null:
                    buffer.Write(s_Null);
                    break;
                default:
                    buffer.Write(s_Undefined);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BuildJsonStringWithFormat(ref RefWriter<char> buffer, string numberFormat = null, IFormatProvider formatProvider = null) =>
            BuildJsonString(ref buffer, TextIndents.GetIndent(0), 0, numberFormat, formatProvider);

        private void BuildJsonString(ref RefWriter<char> buffer, string indent, int indentLevel, string numberFormat, IFormatProvider formatProvider) {
            switch (type) {
                case GJsonType.String:
                    if (m_String == null) buffer.Write(s_Null);
                    else {
                        buffer.Write('"');
                        TextEscape.Exec(m_String, ref buffer);
                        buffer.Write('"');
                    }
                    break;
                case GJsonType.Long:
                    buffer.Write(m_Long.ToString(numberFormat, formatProvider));
                    break;
                case GJsonType.Double:
                    buffer.Write(m_Double.ToString(numberFormat, formatProvider));
                    break;
                case GJsonType.Object when m_Dict.Count == 0:
                    buffer.Write('{', '}');
                    break;
                case GJsonType.Object: {
                    var nextIndentLevel = indentLevel + 1;
                    var childIndent = TextIndents.GetIndent(nextIndentLevel);

                    buffer.Write('{', '\n');
                    foreach (var (key, value) in m_Dict) {
                        if (value.type == GJsonType.Null) continue;
                        buffer.Write(childIndent);
                        buffer.Write('"');
                        TextEscape.Exec(key, ref buffer);
                        buffer.Write('"', ':');
                        value.BuildJsonString(ref buffer, childIndent, nextIndentLevel, numberFormat, formatProvider);
                        buffer.Write(',', '\n');
                    }

                    buffer.length -= 2;
                    buffer.Write('\n');
                    buffer.Write(indent);
                    buffer.Write('}');
                    break;
                }
                case GJsonType.Array when m_List.Count == 0:
                    buffer.Write('[', ']');
                    break;
                case GJsonType.Array: {
                    var nextIndentLevel = indentLevel + 1;
                    var childIndent = TextIndents.GetIndent(nextIndentLevel);

                    buffer.Write('[', '\n');
                    foreach (var item in m_List) {
                        buffer.Write(childIndent);
                        item.BuildJsonString(ref buffer, childIndent, nextIndentLevel, numberFormat, formatProvider);
                        buffer.Write(',', '\n');
                    }

                    buffer.length -= 2;
                    buffer.Write('\n');
                    buffer.Write(indent);
                    buffer.Write(']');
                    break;
                }
                case GJsonType.Boolean:
                    buffer.Write(m_Long != 0 ? s_True : s_False);
                    break;
                case GJsonType.Null:
                    buffer.Write(s_Null);
                    break;
                default:
                    buffer.Write(s_Undefined);
                    break;
            }
        }
    }
}