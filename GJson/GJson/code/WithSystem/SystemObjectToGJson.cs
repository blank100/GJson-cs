using System.Collections;

namespace Gal.Core.GJson
{
	/// <summary>
	/// 将C#原生对象转换为 GJsonObject 对象
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
	public static class SystemObjectToGJson
	{
		public static GJsonObject ToJson(object value) => value switch {
			string v => v,
			int v => v,
			float v => v,
			double v => v,
			IDictionary v => DictionaryToJson(v),
			ICollection v => CollectionToJson(v),
			long v => v,
			bool v => v,
			null => new(GJsonType.Null),
			uint v => v,
			short v => v,
			ushort v => v,
			sbyte v => v,
			byte v => v,
			decimal v => (double)v,
			char v => v,
			_ => throw new($"{value}不能转换为{nameof(GJsonObject)}")
		};

		private static GJsonObject DictionaryToJson(IDictionary value) {
			GJsonObject result = new(GJsonType.Object);
			if (value.Count == 0) return result;

			var itr = value.GetEnumerator();
			while (itr.MoveNext()) {
				var e = itr.Entry;
				if (e.Key is string s) {
					result.Add(s, ToJson(e.Value));
				} else throw new(" JsonObject 的 key 必须时 string ");
			}
			return result;
		}

		private static GJsonObject CollectionToJson(ICollection value) {
			GJsonObject result = new(GJsonType.Array);
			if (value.Count == 0) return result;

			var itr = value.GetEnumerator();
			while (itr.MoveNext()) result.Add(ToJson(itr.Current));
			return result;
		}
	}
}