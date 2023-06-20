using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gal.Core.GJson
{
    /// <summary>
    /// json对象
    /// </summary>
    /// <para>author gouanlin</para>
    public partial class GJsonObject : IDisposable
    {
        public GJsonType type {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set;
        } = GJsonType.Null;

        internal Dictionary<string, GJsonObject> m_Dict;
        internal List<GJsonObject> m_List;
        internal long m_Long;
        internal double m_Double;
        internal string m_String;

        public bool isString {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.String;
        }

        public bool isObject {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.Object;
        }

        public bool isArray {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.Array;
        }

        public bool isNumber {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.Long || type == GJsonType.Double;
        }

        public bool isBoolean {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.Boolean;
        }

        public bool isNull {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.Null;
        }

        public bool isUndefined {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type == GJsonType.Null;
        }

        public int count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type switch {
                GJsonType.Object => m_Dict.Count
                , GJsonType.Array => m_List.Count
                , GJsonType.Null => 0
                , _ => throw new("不是 JsonObject/JsonArray 对象,不能访问元素数量")
            };
        }

        public GJsonObject() { }

        public GJsonObject(GJsonType type) {
            this.type = type;
            switch (type) {
                case GJsonType.Object:
                    m_Dict = new();
                    break;
                case GJsonType.Array:
                    m_List = new();
                    break;
            }
        }

        public GJsonObject(double value) {
            type = GJsonType.Double;
            m_Double = value;
        }

        public GJsonObject(long value) {
            type = GJsonType.Long;
            m_Long = value;
        }

        public GJsonObject(string value) {
            type = GJsonType.String;
            m_String = value;
        }

        public GJsonObject(bool value) {
            type = GJsonType.Boolean;
            m_Long = value ? 1L : 0L;
        }

        public GJsonObject(Dictionary<string, object> dictionary) {
            type = GJsonType.Object;
            m_Dict = new();
            foreach (var (k, v) in dictionary) m_Dict.Add(k, (GJsonObject)v);
        }

        public GJsonObject(Dictionary<string, GJsonObject> dictionary) {
            type = GJsonType.Object;
            m_Dict = new();
            foreach (var (k, v) in dictionary) m_Dict.Add(k, v);
        }

        public GJsonObject(IEnumerable<object> list) {
            type = GJsonType.Array;
            m_List = new();
            foreach (var v in list) m_List.Add((GJsonObject)v);
        }

        public GJsonObject(IEnumerable<GJsonObject> list) {
            type = GJsonType.Array;
            m_List = new();
            m_List.AddRange(list);
        }

        public GJsonObject(ReadOnlySpan<object> span) {
            type = GJsonType.Array;
            m_List = new();
            foreach (var v in span) m_List.Add((GJsonObject)v);
        }

        public GJsonObject(ReadOnlySpan<GJsonObject> span) {
            type = GJsonType.Array;
            m_List = new();
            foreach (var v in span) m_List.Add(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(double value) {
            Clear();
            type = GJsonType.Double;
            m_Double = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(long value) {
            Clear();
            type = GJsonType.Long;
            m_Long = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(string value) {
            Clear();
            type = GJsonType.String;
            m_String = value;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(bool value) {
            Clear();
            type = GJsonType.Boolean;
            m_Long = value ? 1L : 0L;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(Dictionary<string, object> dictionary) {
            Clear();
            type = GJsonType.Object;
            m_Dict = new();
            foreach (var (k, v) in dictionary) m_Dict.Add(k, (GJsonObject)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(Dictionary<string, GJsonObject> dictionary) {
            Clear();
            type = GJsonType.Object;
            m_Dict = new();
            foreach (var (k, v) in dictionary) m_Dict.Add(k, v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(IEnumerable<object> list) {
            Clear();
            type = GJsonType.Array;
            m_List = new();
            foreach (var v in list) m_List.Add((GJsonObject)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(IEnumerable<GJsonObject> list) {
            Clear();
            type = GJsonType.Array;
            m_List = new();
            m_List.AddRange(list);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(ReadOnlySpan<object> list) {
            Clear();
            type = GJsonType.Array;
            m_List = new();
            foreach (var v in list) m_List.Add((GJsonObject)v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Set(ReadOnlySpan<GJsonObject> list) {
            Clear();
            type = GJsonType.Array;
            m_List = new();
            foreach (var v in list) m_List.Add(v);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<GJsonObject> GetList() => type == GJsonType.Array ? m_List : throw new("不是 JsonArray");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<KeyValuePair<string, GJsonObject>> GetDictionary() => type == GJsonType.Object ? m_Dict : throw new("不是 JsonObject");

        public GJsonObject this[string key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Dict.TryGetValue(key, out var t) ? t : null;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if (type == GJsonType.Object) {
                    if (m_Dict.TryGetValue(key, out var oldValue)) {
                        if (ReferenceEquals(oldValue, value)) return;
                        oldValue.Dispose();
                    }

                    if (value == null) {
                        m_Dict.Remove(key);
                        return;
                    }
                } else if (type == GJsonType.Null) {
                    if (value == null) return;
                    type = GJsonType.Object;
                    m_Dict = new();
                } else {
                    if (value != null) throw new("不是 JsonObject ,不能通过 key 设置属性");
                }

                m_Dict[key] = value;
            }
        }

        public GJsonObject this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_List[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                var t = value ?? new();
                switch (type) {
                    case GJsonType.Array when index < m_List.Count:
                        m_List[index]?.Dispose();
                        m_List[index] = t;
                        break;
                    case GJsonType.Array when m_List.Count == index:
                        m_List.Add(t);
                        break;
                    case GJsonType.Array: throw new($"类型为 JsonArray 的索引器({nameof(index)}) 不能大于 {nameof(count)}");
                    case GJsonType.Null when index == 0:
                        type = GJsonType.Array;
                        m_List = new() { t };
                        break;
                    case GJsonType.Null: throw new("未初始化的 JsonArray ,只能设置 index 为 0 的元素值");
                    default: throw new("不是 JsonArray ,不能通过 index 设置元素值");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(GJsonObject item) {
            switch (type) {
                case GJsonType.Array:
                    m_List.Add(item ?? new());
                    break;
                case GJsonType.Null:
                    type = GJsonType.Array;
                    m_List = new() { item ?? new() };
                    break;
                default: throw new($"不是 JsonArray ,不能通过 {nameof(Add)}(GJsonObject item) 添加元素值");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string key, GJsonObject value) {
            switch (type) {
                case GJsonType.Object:
                    m_Dict.Add(key, value ?? new());
                    break;
                case GJsonType.Null:
                    type = GJsonType.Object;
                    m_Dict = new() { { key, value ?? new() } };
                    break;
                default: throw new($"不是 JsonObject ,不能通过 {nameof(Add)}(string key, GJsonObject value) 添加属性");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasChild(string key) { return type switch { GJsonType.Object => m_Dict.ContainsKey(key), GJsonType.Null => false, _ => throw new($"不是 JsonObject ,不能通过 {nameof(HasChild)}(string key) 判断是否有指定的子节点") }; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetChild(string key, out GJsonObject value) => type == GJsonType.Object ? m_Dict.TryGetValue(key, out value) : throw new($"不是 JsonObject ,不能通过 {nameof(TryGetChild)}(string key,out GJsonObject value) 获取值");

        public GJsonObject Clear() {
            type = GJsonType.Null;

            if (m_Dict != null) {
                m_Dict.Clear();
                m_Dict = null;
            }

            if (m_List != null) {
                m_List.Clear();
                m_List = null;
            }

            m_Long = default;
            m_Double = default;
            m_String = default;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GJsonObject Clone() {
            GJsonObject t = new(type);
            t.m_Dict = m_Dict;
            t.m_List = m_List;
            t.m_Long = m_Long;
            t.m_Double = m_Double;
            t.m_String = m_String;
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
        public static explicit operator int(GJsonObject value) => value == null ? default : value.type is GJsonType.Long ? (int)value.m_Long : value.type is GJsonType.Double ? (int)value.m_Double : throw new($"{value.type} 的 json 对象不能转换为 {nameof(Int32)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(GJsonObject value) => value == null ? default : value.type is GJsonType.Long ? value.m_Long : value.type is GJsonType.Double ? (long)value.m_Double : throw new($"{value.type} 的 json 对象不能转换为 {nameof(Int64)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(GJsonObject value) => value == null ? default : value.type is GJsonType.Long ? value.m_Long : value.type is GJsonType.Double ? value.m_Double : throw new($"{value.type} 的 json 对象不能转换为 {nameof(Double)}");

        /// <summary>
        /// 非隐式转换,因为可能会存在数据截断
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(GJsonObject value) => value == null ? default : value.type is GJsonType.Long ? value.m_Long : value.type is GJsonType.Double ? (float)value.m_Double : throw new($"{value.type} 的 json 对象不能转换为 {nameof(Single)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(GJsonObject value) => value == null ? default : value.type == GJsonType.String ? value.m_String : value.ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(GJsonObject value) => value == null ? default : value.type == GJsonType.Boolean ? value.m_Long != 0 : throw new($"{value.type} 的 json 对象不能转换为 {nameof(Boolean)}");

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