using System;

namespace Gal.Core.GJson
{
	/// <summary>
	/// GJsonObject 二进制序列化
	/// </summary>
	/// <para>author gouanlin</para>
	public static class GJsonBinary
	{
		public static void Encode(GJsonObject json, ref RefWriter<byte> writer) {
			var type = json.type;
			switch (type) {
				case GJsonType.Long: {
						WriteVarLong(ref writer, type, json.m_Long);
						break;
					}
				case GJsonType.String: {
						var text = json.m_String;
						var textUtf8ByteCount = System.Text.Encoding.UTF8.GetByteCount(text);
						WriteVarLength(ref writer, type, textUtf8ByteCount);
						writer.WriteUtf8(text, textUtf8ByteCount);
						break;
					}
				case GJsonType.Object: {
						WriteVarLength(ref writer, type, json.m_Dict.Count);
						foreach (var (key, value) in json.m_Dict) {
							var textUtf8ByteCount = System.Text.Encoding.UTF8.GetByteCount(key);
							writer.WriteVarUInt32((uint)textUtf8ByteCount);
							writer.WriteUtf8(key, textUtf8ByteCount);
							Encode(value, ref writer);
						}
						break;
					}
				case GJsonType.Array: {
						WriteVarLength(ref writer, type, json.m_List.Count);
						foreach (var value in json.m_List) Encode(value, ref writer);
						break;
					}
				case GJsonType.Double:
					writer.Write((byte)type);
					writer.WriteDouble(json.m_Double);
					break;
				case GJsonType.Null:
					writer.Write((byte)type);
					break;
				case GJsonType.Boolean:
					writer.Write((byte)((int)type | ((json.m_Long != 0) ? 1 << 3 : 0)));
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public static GJsonObject Decode(ref RefReader<byte> reader) {
			var header = reader.Read();
			var type = (GJsonType)(header & 0b00000111);
			switch (type) {
				case GJsonType.Long: return GJsonObject.Get(ReadVarLong(ref reader, header >> 3));
				case GJsonType.String: return GJsonObject.Get(reader.ReadUtf8(ReadVarLength(ref reader, header)));
				case GJsonType.Null: return GJsonObject.Get(GJsonType.Null);
				case GJsonType.Object: {
						var json = GJsonObject.Get(GJsonType.Object);
						var count = ReadVarLength(ref reader, header);
						for (var i = 0; i < count; i++) {
							var keyUtf8ByteCount = reader.ReadVarUInt32();
							json.Add(reader.ReadUtf8((int)keyUtf8ByteCount), Decode(ref reader));
						}
						return json;
					}
				case GJsonType.Array: {
						var json = GJsonObject.Get(GJsonType.Array);
						var count = ReadVarLength(ref reader, header);
						for (var i = 0; i < count; i++) json.Add(Decode(ref reader));
						return json;
					}
				case GJsonType.Double: return GJsonObject.Get(reader.ReadDouble());
				case GJsonType.Boolean: return GJsonObject.Get(header >> 3 != 0);
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public static void WriteVarLong(ref RefWriter<byte> writer, GJsonType type, long value) {
			var span = writer.GetSpan(1);
			writer.Advance(1);
			var p = writer.position;

			var v = ZigZagUtils.EncodeZigZag64(value);
			while (v >= byte.MaxValue) {
				writer.Write((byte)v);
				v >>= 8;
			}
			writer.Write((byte)v);

			span[0] = (byte)((int)type | ((writer.position - p) << 3));
		}

		public static long ReadVarLong(ref RefReader<byte> reader, int bytesCount) {
			ulong v = reader.Read();
			for (int shift = 8, m = bytesCount * 8; shift < m; shift += 8) v |= (ulong)reader.Read() << shift;
			return ZigZagUtils.DecodeZigZag64(v);
		}

		public static void WriteVarLength(ref RefWriter<byte> writer, GJsonType type, int length) {
			if (length < 0b00001111) writer.Write((byte)((int)type | (length << 3)));
			else {
				writer.Write((byte)((int)type | (length << 3) | 0b10000000));
				length >>= 3;
				for (; length >= 0b10000000;) {
					writer.Write((byte)(length | 0b10000000));
					length >>= 7;
				}
				writer.Write((byte)length);
			}
		}

		public static int ReadVarLength(ref RefReader<byte> reader, byte firstByte) {
			var value = 0;

			var t = firstByte;
			value |= (t & 0b01111000) >> 3;
			if ((t & 0b10000000) == 0) return value;

			for (var shift = 3; shift <= 32; shift += 7) {
				t = reader.Read();
				value |= (t & 0b01111111) << shift;
				if ((t & 0b10000000) == 0) return value;
			}

			return value;
		}
	}
}