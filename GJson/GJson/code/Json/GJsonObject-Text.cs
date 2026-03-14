using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
	/// <summary>
	/// json对象
	/// </summary>
	/// <author>gouanlin</author>
	public partial class GJsonObject
	{
		private static readonly ReadOnlyMemory<char> Null = "null".AsMemory();
		private static readonly ReadOnlyMemory<char> True = "true".AsMemory();
		private static readonly ReadOnlyMemory<char> False = "false".AsMemory();
		private static readonly ReadOnlyMemory<char> Undefined = "undefined".AsMemory();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => ToString(false, null, CultureInfo.InvariantCulture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(bool format, string numberFormat = null, IFormatProvider formatProvider = null) {
			RefWriter<char> buffer = new(stackalloc char[256]);
			try {
				if (format) BuildJsonStringWithFormat(ref buffer, numberFormat, formatProvider ?? CultureInfo.InvariantCulture);
				else BuildJsonString(ref buffer, numberFormat, formatProvider ?? CultureInfo.InvariantCulture);
				return buffer.WrittenSpan.ToString();
			} finally {
				buffer.Dispose();
			}
		}

		private void BuildJsonString(ref RefWriter<char> buffer, string numberFormat, IFormatProvider formatProvider) {
			switch (Type) {
				case GJsonType.String:
					WriteString(ref buffer, String);
					break;
				case GJsonType.Long:
					buffer.Write(Long.ToString(numberFormat, formatProvider));
					break;
				case GJsonType.Double:
					buffer.Write(Double.ToString(numberFormat, formatProvider));
					break;
				case GJsonType.Object when Dict.Count == 0:
					buffer.Write('{', '}');
					break;
				case GJsonType.Object: {
					buffer.Write('{');
					foreach (var (key, value) in Dict) {
						buffer.Write('"');
						TextEscape.Exec(key, ref buffer);
						buffer.Write('"', ':');
						value.BuildJsonString(ref buffer, numberFormat, formatProvider);
						buffer.Write(',');
					}

					buffer.Length--;
					buffer.Write('}');
					break;
				}
				case GJsonType.Array when List.Count == 0:
					buffer.Write('[', ']');
					break;
				case GJsonType.Array: {
					buffer.Write('[');
					foreach (var item in List) {
						item.BuildJsonString(ref buffer, numberFormat, formatProvider);
						buffer.Write(',');
					}

					buffer.Length--;
					buffer.Write(']');
					break;
				}
				case GJsonType.Boolean:
					buffer.Write(Long != 0 ? True : False);
					break;
				case GJsonType.Null:
					buffer.Write(Null);
					break;
				default:
					buffer.Write(Undefined);
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void BuildJsonStringWithFormat(ref RefWriter<char> buffer, string numberFormat = null, IFormatProvider formatProvider = null) =>
			BuildJsonString(ref buffer, TextIndents.GetIndent(0), 0, numberFormat, formatProvider);

		private void BuildJsonString(ref RefWriter<char> buffer, string indent, int indentLevel, string numberFormat, IFormatProvider formatProvider) {
			switch (Type) {
				case GJsonType.String:
					WriteString(ref buffer, String);
					break;
				case GJsonType.Long: {
					Span<char> longBuffer = stackalloc char[32];
					Long.TryFormat(longBuffer, out var written, numberFormat, formatProvider);
					buffer.HintSize(written);
					longBuffer[..written].CopyTo(buffer.Span);
					buffer.Advance(written);
				}
					// buffer.Write(m_Long.ToString(numberFormat, formatProvider));
					break;
				case GJsonType.Double: {
					Span<char> doubleBuffer = stackalloc char[64];
					Double.TryFormat(doubleBuffer, out var written, numberFormat, formatProvider);
					buffer.HintSize(written);
					doubleBuffer[..written].CopyTo(buffer.Span);
					buffer.Advance(written);
				}
					// buffer.Write(m_Double.ToString(numberFormat, formatProvider));
					break;
				case GJsonType.Object when Dict.Count == 0:
					buffer.Write('{', '}');
					break;
				case GJsonType.Object: {
					var nextIndentLevel = indentLevel + 1;
					var childIndent = TextIndents.GetIndent(nextIndentLevel);

					buffer.Write('{', '\n');
					foreach (var (key, value) in Dict) {
						if (value.Type == GJsonType.Null) continue;
						buffer.Write(childIndent);
						buffer.Write('"');
						TextEscape.Exec(key, ref buffer);
						buffer.Write('"', ':');
						value.BuildJsonString(ref buffer, childIndent, nextIndentLevel, numberFormat, formatProvider);
						buffer.Write(',', '\n');
					}

					buffer.Length -= 2;
					buffer.Write('\n');
					buffer.Write(indent);
					buffer.Write('}');
					break;
				}
				case GJsonType.Array when List.Count == 0:
					buffer.Write('[', ']');
					break;
				case GJsonType.Array: {
					var nextIndentLevel = indentLevel + 1;
					var childIndent = TextIndents.GetIndent(nextIndentLevel);

					buffer.Write('[', '\n');
					foreach (var item in List) {
						buffer.Write(childIndent);
						item.BuildJsonString(ref buffer, childIndent, nextIndentLevel, numberFormat, formatProvider);
						buffer.Write(',', '\n');
					}

					buffer.Length -= 2;
					buffer.Write('\n');
					buffer.Write(indent);
					buffer.Write(']');
					break;
				}
				case GJsonType.Boolean:
					buffer.Write(Long != 0 ? True : False);
					break;
				case GJsonType.Null:
					buffer.Write(Null);
					break;
				default:
					buffer.Write(Undefined);
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteString(ref RefWriter<char> buffer, string value) {
			if (value == null) {
				buffer.Write(Null);
				return;
			}
			buffer.Write('"');
			TextEscape.Exec(value, ref buffer);
			buffer.Write('"');
		}
	}
}
