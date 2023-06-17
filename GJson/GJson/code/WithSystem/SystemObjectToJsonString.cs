using System.Collections;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    /// <summary>
    /// 将C#原生对象转换为 Json 字符串
    /// <para>author gouanlin</para>
    /// <para>IDictionary = JsonObject</para>
    /// <para>ICollection = JsonArray</para>
    /// <para>string = string</para>
    /// <para>int = number</para>
    /// <para>float = number</para>
    /// <para>double = number</para>
    /// <para>long = number</para>
    /// <para>bool = bool</para>
    /// <para>null = null</para>
    /// <para>uint = number</para>
    /// <para>short = number</para>
    /// <para>ushort = number</para>
    /// <para>sbyte = number</para>
    /// <para>byte = number</para>
    /// <para>decimal = number</para>
    /// </summary>
    public static class SystemObjectToJsonString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToJsonString(this object self, bool withFormat = false, string numberFormat = null) {
            RefWriter<char> writer = new();
            try {
                if (withFormat) Object2Json(self, ref writer, numberFormat, TextIndents.GetIndent(0), 1);
                else Object2Json(self, ref writer, numberFormat, null, 0);
                return writer.writtenSpan.ToString();
            } finally {
                writer.Dispose();
            }
        }

        private static void Object2Json(object value, ref RefWriter<char> writer, string numberFormat, string indent, int indentLevel) {
            switch (value) {
                case string stringValue:
                    writer.Write('"');
                    TextEscape.Exec(stringValue, ref writer);
                    writer.Write('"');
                    break;
                case int intValue:
                    writer.Write(intValue.ToString(numberFormat));
                    break;
                case float floatValue:
                    writer.Write(floatValue.ToString(numberFormat));
                    break;
                case double doubleValue:
                    writer.Write(doubleValue.ToString(numberFormat));
                    break;
                case IDictionary jsonObject:
                    Dictionary2Json(jsonObject, ref writer, numberFormat, indent, indentLevel);
                    break;
                case ICollection jsonArray:
                    Collection2Json(jsonArray, ref writer, numberFormat, indent, indentLevel);
                    break;
                case long longValue:
                    writer.Write(longValue.ToString(numberFormat));
                    break;
                case bool boolValue:
                    writer.Write(boolValue ? "true" : "false");
                    break;
                case null:
                    writer.Write("null");
                    break;
                case uint uintValue:
                    writer.Write(uintValue.ToString(numberFormat));
                    break;
                case short shortValue:
                    writer.Write(shortValue.ToString(numberFormat));
                    break;
                case ushort ushortValue:
                    writer.Write(ushortValue.ToString(numberFormat));
                    break;
                case sbyte sbyteValue:
                    writer.Write(sbyteValue.ToString(numberFormat));
                    break;
                case byte byteValue:
                    writer.Write(byteValue.ToString(numberFormat));
                    break;
                case decimal decimalValue:
                    writer.Write(decimalValue.ToString(numberFormat));
                    break;
                default:
                    writer.Write(value.ToString());
                    break;
            }
        }

        private static void Collection2Json(ICollection value, ref RefWriter<char> writer, string numberFormat, string indent, int indentLevel) {
            if (value.Count == 0) {
                writer.Write('[', ']');
                return;
            }
            if (string.IsNullOrEmpty(indent)) {
                writer.Write('[');
                var itr = value.GetEnumerator();
                while (itr.MoveNext()) {
                    Object2Json(itr.Current, ref writer, numberFormat, indent, indentLevel);
                    writer.Write(',');
                }
                writer.length--;
            } else {
                var nextIndentLevel = indentLevel + 1;
                var childIndent = TextIndents.GetIndent(nextIndentLevel);

                writer.Write('[', '\n');
                var itr = value.GetEnumerator();
                while (itr.MoveNext()) {
                    writer.Write(childIndent);
                    Object2Json(itr.Current, ref writer, numberFormat, childIndent, nextIndentLevel);
                    writer.Write(',', '\n');
                }
                writer.length -= 2;
                writer.Write('\n');
                writer.Write(indent);
            }
            writer.Write(']');
        }

        private static void Dictionary2Json(IDictionary value, ref RefWriter<char> writer, string numberFormat, string indent, int indentLevel) {
            if (value.Count == 0) {
                writer.Write('{', '}');
                return;
            }
            if (string.IsNullOrEmpty(indent)) {
                writer.Write('{');
                var itr = value.GetEnumerator();
                while (itr.MoveNext()) {
                    var e = itr.Entry;

                    writer.Write('"');
                    TextEscape.Exec(e.Key.ToString(), ref writer);
                    writer.Write('"', ':');
                    Object2Json(e.Value, ref writer, numberFormat, indent, indentLevel);
                    writer.Write(',');
                }
                writer.length--;
            } else {
                var nextIndentLevel = indentLevel + 1;
                var childIndent = TextIndents.GetIndent(nextIndentLevel);

                writer.Write('{', '\n');
                var itr = value.GetEnumerator();
                while (itr.MoveNext()) {
                    var e = itr.Entry;

                    writer.Write(childIndent + '"');
                    TextEscape.Exec(e.Key.ToString(), ref writer);
                    writer.Write('"', ':');
                    Object2Json(e.Value, ref writer, numberFormat, childIndent, nextIndentLevel);
                    writer.Write(',', '\n');
                }
                writer.length -= 2;
                writer.Write('\n');
                writer.Write(indent);
            }
            writer.Write('}');
        }
    }
}