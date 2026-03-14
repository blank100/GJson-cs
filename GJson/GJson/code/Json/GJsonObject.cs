using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    /// <summary>
    /// json对象
    /// </summary>
    /// <author>gouanlin</author>
    public partial class GJsonObject : IDisposable
    {
        public GJsonType Type {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set;
        } = GJsonType.Null;

        internal Dictionary<string, GJsonObject> Dict;
        internal List<GJsonObject> List;
        internal long Long;
        internal double Double;
        internal string String;

        public bool IsString {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type == GJsonType.String;
        }

        public bool IsObject {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type == GJsonType.Object;
        }

        public bool IsArray {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type == GJsonType.Array;
        }

        public bool IsNumber {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type is GJsonType.Long or GJsonType.Double;
        }

        public bool IsBoolean {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type == GJsonType.Boolean;
        }

        public bool IsNull {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type == GJsonType.Null;
        }

        public bool IsUndefined {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type == GJsonType.Null;
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Type switch {
                GJsonType.Object => Dict.Count
                , GJsonType.Array => List.Count
                , GJsonType.Null => 0
                , _ => throw new("不是 JsonObject/JsonArray 对象,不能访问元素数量")
            };
        }

        public GJsonObject() { }

        public GJsonObject(GJsonType type) {
            this.Type = type;
            switch (type) {
                case GJsonType.Object:
                    Dict = new();
                    break;
                case GJsonType.Array:
                    List = new();
                    break;
            }
        }

        public GJsonObject(double value) {
            Type = GJsonType.Double;
            Double = value;
        }

        public GJsonObject(long value) {
            Type = GJsonType.Long;
            Long = value;
        }

        public GJsonObject(string value) {
            Type = GJsonType.String;
            String = value;
        }

        public GJsonObject(bool value) {
            Type = GJsonType.Boolean;
            Long = value ? 1L : 0L;
        }

        public GJsonObject(Dictionary<string, object> dictionary) {
            Type = GJsonType.Object;
            Dict = new();
            foreach (var (k, v) in dictionary) Dict.Add(k, (GJsonObject)v);
        }

        public GJsonObject(Dictionary<string, GJsonObject> dictionary) {
            Type = GJsonType.Object;
            Dict = new();
            foreach (var (k, v) in dictionary) Dict.Add(k, v);
        }

        public GJsonObject(IEnumerable<object> list) {
            Type = GJsonType.Array;
            List = new();
            foreach (var v in list) List.Add((GJsonObject)v);
        }

        public GJsonObject(IEnumerable<GJsonObject> list) {
            Type = GJsonType.Array;
            List = new();
            List.AddRange(list);
        }

        public GJsonObject(ReadOnlySpan<object> span) {
            Type = GJsonType.Array;
            List = new();
            foreach (var v in span) List.Add((GJsonObject)v);
        }

        public GJsonObject(ReadOnlySpan<GJsonObject> span) {
            Type = GJsonType.Array;
            List = new();
            foreach (var v in span) List.Add(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(double value) {
            Clear();
            Type = GJsonType.Double;
            Double = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(long value) {
            Clear();
            Type = GJsonType.Long;
            Long = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(string value) {
            Clear();
            Type = GJsonType.String;
            String = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(bool value) {
            Clear();
            Type = GJsonType.Boolean;
            Long = value ? 1L : 0L;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(Dictionary<string, object> dictionary) {
            Clear();
            Type = GJsonType.Object;
            Dict = new();
            foreach (var (k, v) in dictionary) Dict.Add(k, (GJsonObject)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(Dictionary<string, GJsonObject> dictionary) {
            Clear();
            Type = GJsonType.Object;
            Dict = new();
            foreach (var (k, v) in dictionary) Dict.Add(k, v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(IEnumerable<object> list) {
            Clear();
            Type = GJsonType.Array;
            List = new();
            foreach (var v in list) List.Add((GJsonObject)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(IEnumerable<GJsonObject> list) {
            Clear();
            Type = GJsonType.Array;
            List = new();
            List.AddRange(list);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(ReadOnlySpan<object> list) {
            Clear();
            Type = GJsonType.Array;
            List = new();
            foreach (var v in list) List.Add((GJsonObject)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(ReadOnlySpan<GJsonObject> list) {
            Clear();
            Type = GJsonType.Array;
            List = new();
            foreach (var v in list) List.Add(v);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<GJsonObject> GetList() => Type == GJsonType.Array ? List : throw new("不是 JsonArray");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyDictionary<string, GJsonObject> GetDictionary() => Type == GJsonType.Object ? Dict : throw new("不是 JsonObject");

        public GJsonObject this[string key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                Debug.Assert(Type == GJsonType.Object, "不是 JsonObject ,不能通过 key 获取属性");
                return Dict.GetValueOrDefault(key);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if (Type == GJsonType.Object) {
                    if (Dict.TryGetValue(key, out var oldValue)) {
                        if (ReferenceEquals(oldValue, value)) return;
                        oldValue.Dispose();
                    }
                } else if (Type == GJsonType.Null) {
                    Type = GJsonType.Object;
                    Dict = new();
                } else throw new("不是 JsonObject ,不能通过 key 设置属性");

                Dict[key] = value ?? new();
            }
        }

        public GJsonObject this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                Debug.Assert(Type == GJsonType.Array, "不是 JsonArray ,不能通过 index 获取属性");
                Debug.Assert(index >= 0 && index < List.Count, $"类型为 JsonArray 的索引器({nameof(index)}) 不能大于 {nameof(Count)}");
                return List[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                switch (Type) {
                    case GJsonType.Array when index >= 0 && index < List.Count: {
                        var oldValue = List[index];
                        if (ReferenceEquals(oldValue, value)) return;
                        oldValue.Dispose();
                        List[index] = value ?? new();
                        break;
                    }
                    case GJsonType.Array when index == List.Count: {
                        List.Add(value ?? new());
                        break;
                    }
                    case GJsonType.Array: throw new($"类型为 JsonArray 的索引器({nameof(index)}) 不能大于 {nameof(Count)}");
                    case GJsonType.Null when index == 0: {
                        Type = GJsonType.Array;
                        List = new() { value ?? new() };
                        break;
                    }
                    case GJsonType.Null: throw new("未初始化的 JsonArray ,只能设置 index 为 0 的元素值");
                    default: throw new("不是 JsonArray ,不能通过 index 设置元素值");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(GJsonObject item) {
            switch (Type) {
                case GJsonType.Array:
                    List.Add(item ?? new());
                    break;
                case GJsonType.Null:
                    Type = GJsonType.Array;
                    List = new() { item ?? new() };
                    break;
                default: throw new($"不是 JsonArray ,不能通过 {nameof(Add)}(GJsonObject item) 添加元素值");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int index, GJsonObject item) {
            switch (Type) {
                case GJsonType.Array:
                    List.Insert(index, item ?? new());
                    break;
                case GJsonType.Null when index == 0:
                    Type = GJsonType.Array;
                    List = new() { item ?? new() };
                    break;
            }
            throw new($"不是 JsonArray ,不能通过 {nameof(Add)}(int index, GJsonObject item) 添加元素值");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, GJsonObject item) => Add(index, item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string key, GJsonObject value) {
            switch (Type) {
                case GJsonType.Object:
                    Dict.Add(key, value ?? new());
                    break;
                case GJsonType.Null:
                    Type = GJsonType.Object;
                    Dict = new() { { key, value ?? new() } };
                    break;
                default: throw new($"不是 JsonObject ,不能通过 {nameof(Add)}(string key, GJsonObject value) 添加属性");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int index) {
            if (Type != GJsonType.Array) throw new($"不是 JsonArray ,不能通过 {nameof(Remove)}(int index) 移除元素");
            if (index >= 0 && index < List.Count) List.RemoveAt(index);
            else throw new ArgumentOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(string key) {
            if (Type != GJsonType.Object) throw new($"不是 JsonObject ,不能通过 {nameof(Remove)}(string key) 移除元素");
            Dict.Remove(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasChild(string key) {
            return Type switch {
                GJsonType.Object => Dict.ContainsKey(key)
                , GJsonType.Null => false
                , _ => throw new($"不是 JsonObject ,不能通过 {nameof(HasChild)}(string key) 判断是否有指定的子节点")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetChild(string key, out GJsonObject value) => Type == GJsonType.Object ?
            Dict.TryGetValue(key, out value) :
            throw new($"不是 JsonObject ,不能通过 {nameof(TryGetChild)}(string key,out GJsonObject value) 获取值");

        public GJsonObject Clear() {
            Type = GJsonType.Null;

            if (Dict != null) {
                Dict.Clear();
                Dict = null;
            }

            if (List != null) {
                List.Clear();
                List = null;
            }

            Long = default;
            Double = default;
            String = default;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Clone() {
            GJsonObject t = new(Type);

            if (Dict != null) {
                t.Dict = new(Dict.Count);
                foreach (var (k, v) in Dict) t.Dict.Add(k, v.Clone());
            }
            if (List != null) {
                t.List = new(List.Count);
                for (int i = 0, l = List.Count; i < l; i++) t.List[i] = List[i].Clone();
            }

            t.Long = Long;
            t.Double = Double;
            t.String = String;
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => Clear();

        /// <summary>
        /// 非隐式转换,因为可能会存在数据截断
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(GJsonObject value) => value == null ? default :
            value.Type is GJsonType.Long ? (int)value.Long :
            value.Type is GJsonType.Double ? (int)value.Double : throw new($"{value.Type} 的 json 对象不能转换为 {nameof(Int32)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(GJsonObject value) => value == null ? default :
            value.Type is GJsonType.Long ? value.Long :
            value.Type is GJsonType.Double ? (long)value.Double : throw new($"{value.Type} 的 json 对象不能转换为 {nameof(Int64)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(GJsonObject value) => value == null ? default :
            value.Type is GJsonType.Long ? value.Long :
            value.Type is GJsonType.Double ? value.Double : throw new($"{value.Type} 的 json 对象不能转换为 {nameof(System.Double)}");

        /// <summary>
        /// 非隐式转换,因为可能会存在数据截断
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(GJsonObject value) => value == null ? default :
            value.Type is GJsonType.Long ? value.Long :
            value.Type is GJsonType.Double ? (float)value.Double : throw new($"{value.Type} 的 json 对象不能转换为 {nameof(Single)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(GJsonObject value) => value == null ? default : value.Type == GJsonType.String ? value.String : value.ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(GJsonObject value) =>
            value == null ? default : value.Type == GJsonType.Boolean ? value.Long != 0 : throw new($"{value.Type} 的 json 对象不能转换为 {nameof(Boolean)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GJsonObject(int value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GJsonObject(long value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GJsonObject(double value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GJsonObject(string value) {
            if (value == null) return new(GJsonType.Null);
            return new(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GJsonObject(bool value) => new(value);
    }
}
