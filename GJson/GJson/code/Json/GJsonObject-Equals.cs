namespace Gal.Core.GJson
{
    /// <summary>
    /// json对象
    /// </summary>
    /// <author>gouanlin</author>
    public partial class GJsonObject
    {
        public override bool Equals(object obj) {
            if (obj is not GJsonObject other) return false;
            if (Type != other.Type) return false;

            switch (Type) {
                case GJsonType.Null: return true;
                case GJsonType.Boolean: return (Long != 0) == (other.Long != 0);
                case GJsonType.Long: return Long == other.Long;
                case GJsonType.Double: return Number.Equals(Double, other.Double);
                case GJsonType.String: return String == other.String;

                case GJsonType.Array when List.Count != other.List.Count: return false;
                case GJsonType.Array: {
                    for (int i = 0, l = List.Count; i < l; i++) {
                        if (!List[i].Equals(other.List[i])) return false;
                    }
                    break;
                }

                case GJsonType.Object when Dict.Count != other.Dict.Count: return false;
                case GJsonType.Object: {
                    foreach (var (k, v) in Dict) {
                        if (!other.Dict.TryGetValue(k, out var otherValue)) return false;
                        if (!v.Equals(otherValue)) return false;
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        public override int GetHashCode() {
            HashCode hash = new();
            hash.Add(Type);
            hash.Add(Dict);
            hash.Add(List);
            hash.Add(Long);
            hash.Add(Double);
            hash.Add(String);
            hash.Add(Count);
            return hash.ToHashCode();
        }
    }
}