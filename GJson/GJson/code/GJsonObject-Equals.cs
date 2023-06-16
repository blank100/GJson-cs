using System;

namespace Gal.Core.GJson
{
	/// <summary>
	/// json对象
	/// </summary>
	/// <para>author gouanlin</para>
	public partial class GJsonObject
	{
		public override bool Equals(object obj) {
			if (obj is not GJsonObject other) {
				return false;
			}
			if (type != other.type) {
				return false;
			}

			switch (type) {
				case GJsonType.Null: return true;
				case GJsonType.Boolean: return (m_Long != 0) == (other.m_Long != 0);
				case GJsonType.Long: return m_Long == other.m_Long;
				case GJsonType.Double: return Number.Equals(m_Double, other.m_Double);
				case GJsonType.String: return m_String == other.m_String;

				case GJsonType.Array when m_List.Count != other.m_List.Count: return false;
				case GJsonType.Array: {
						for (int i = 0, l = m_List.Count; i < l; i++) {
							if (!m_List[i].Equals(other.m_List[i])) {
								return false;
							}
						}
						break;
					}

				case GJsonType.Object when m_Dict.Count != other.m_Dict.Count: return false;
				case GJsonType.Object: {
						foreach (var (k, v) in m_Dict) {
							if (!other.m_Dict.TryGetValue(k, out var otherValue)) {
								return false;
							}
							if (!v.Equals(otherValue)) {
								return false;
							}
						}
						break;
					}
				default: throw new ArgumentOutOfRangeException();
			}

			return true;
		}

		public override int GetHashCode() {
			HashCode hash = new();
			hash.Add(type);
			hash.Add(m_Dict);
			hash.Add(m_List);
			hash.Add(m_Long);
			hash.Add(m_Double);
			hash.Add(m_String);
			hash.Add(isString);
			hash.Add(isObject);
			hash.Add(isArray);
			hash.Add(isNumber);
			hash.Add(isBoolean);
			hash.Add(isNull);
			hash.Add(isUndefined);
			hash.Add(count);
			return hash.ToHashCode();
		}
	}
}