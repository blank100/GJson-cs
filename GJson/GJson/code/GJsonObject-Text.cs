using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    /// <summary>
    /// json对象
    /// </summary>
    /// <para>author gouanlin</para>
    public partial class GJsonObject
    {
        public static readonly Dictionary<char, string> defaultEscapeTable = new() {
            ['\\'] = "\\\\"
            , ['\"'] = "\\\""
            , ['\n'] = "\\n"
            , ['\r'] = "\\r"
            , ['\t'] = "\\t"
            , ['\b'] = "\\b"
            , ['\f'] = "\\f"
        };

        public override string ToString() {
            RefWriter<char> buffer = new(stackalloc char[256]);
            try {
                BuildJsonString(ref buffer, null);
                return buffer.writtenSpan.ToString();
            } finally {
                buffer.Dispose();
            }
        }

        public string ToString(bool format, string numberFormat = null) {
            RefWriter<char> buffer = new(stackalloc char[256]);
            try {
                if (format) BuildJsonStringWithFormat(ref buffer);
                else BuildJsonString(ref buffer, numberFormat);
                return buffer.writtenSpan.ToString();
            } finally {
                buffer.Dispose();
            }
        }

        private void BuildJsonString(ref RefWriter<char> buffer, string numberFormat) {
            switch (type) {
                case GJsonType.String:
                    if (m_String == null) buffer.Write("null");
                    else {
                        buffer.Write('"');
                        TextEscape.Exec(m_String, ref buffer, defaultEscapeTable, false);
                        buffer.Write('"');
                    }
                    break;
                case GJsonType.Long:
                    buffer.Write(m_Long.ToString(numberFormat));
                    break;
                case GJsonType.Double:
                    buffer.Write(m_Double.ToString(numberFormat));
                    break;
                case GJsonType.Object when m_Dict.Count == 0:
                    buffer.Write('{', '}');
                    break;
                case GJsonType.Object: {
                    buffer.Write('{');
                    foreach (var (key, value) in m_Dict) {
                        buffer.Write('"');
                        TextEscape.Exec(key, ref buffer, defaultEscapeTable, false);
                        buffer.Write('"', ':');
                        value.BuildJsonString(ref buffer, numberFormat);
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
                        item.BuildJsonString(ref buffer, numberFormat);
                        buffer.Write(',');
                    }

                    buffer.length--;
                    buffer.Write(']');
                    break;
                }
                case GJsonType.Boolean:
                    buffer.Write(m_Long != 0 ? "true" : "false");
                    break;
                case GJsonType.Null:
                    buffer.Write("null");
                    break;
                default:
                    buffer.Write("undefined");
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BuildJsonStringWithFormat(ref RefWriter<char> buffer, string numberFormat = null) => BuildJsonString(ref buffer, TextIndents.GetIndent(0), 0, numberFormat);

        private void BuildJsonString(ref RefWriter<char> buffer, string indent, int indentLevel, string numberFormat = null) {
            switch (type) {
                case GJsonType.String:
                    if (m_String == null) buffer.Write("null");
                    else {
                        buffer.Write('"');
                        TextEscape.Exec(m_String, ref buffer, defaultEscapeTable, false);
                        buffer.Write('"');
                    }
                    break;
                case GJsonType.Long:
                    buffer.Write(m_Long.ToString(numberFormat));
                    break;
                case GJsonType.Double:
                    buffer.Write(m_Double.ToString(numberFormat));
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
                        TextEscape.Exec(key, ref buffer, defaultEscapeTable, false);
                        buffer.Write('"', ':');
                        value.BuildJsonString(ref buffer, childIndent, nextIndentLevel);
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
                        item.BuildJsonString(ref buffer, childIndent, nextIndentLevel);
                        buffer.Write(',', '\n');
                    }

                    buffer.length -= 2;
                    buffer.Write('\n');
                    buffer.Write(indent);
                    buffer.Write(']');
                    break;
                }
                case GJsonType.Boolean:
                    buffer.Write(m_Long != 0 ? "true" : "false");
                    break;
                case GJsonType.Null:
                    buffer.Write("null");
                    break;
                default:
                    buffer.Write("undefined");
                    break;
            }
        }
    }
}