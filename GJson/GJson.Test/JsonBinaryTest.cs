using System;

using Gal.Core;
using Gal.Core.GJson;

using Xunit;

namespace GalCoreUnitTest.Classes {
	public class JsonBinaryTest {
		[Fact]
		public void VarLengthTest() {
			(int min, int max)[] list = {
				new(byte.MinValue, byte.MaxValue),
				new(short.MaxValue  - 100, short.MaxValue  + 100),
				new(ushort.MaxValue - 100, ushort.MaxValue + 100),
				new(int.MaxValue    - 100, int.MaxValue),
			};

			Span<byte> buffer = stackalloc byte[512];

			//Uint32测试
			foreach (var (min, max) in list) {
				for (var i = min; i < max; i += 10) {
					RefWriter<byte> writer = new(buffer);
					foreach (GJsonType type in Enum.GetValues(typeof(GJsonType))) {
						for (var j = i; j < i + 10; j++) {
							GJsonBinary.WriteVarLength(ref writer, type, j);

							// var span = writer.GetSpan(5);
							// span[0] = (byte) type;
							// GJsonBinary.WriteVarLength(ref span, j, out var bytesCount);
							// writer.Advance(bytesCount);
						}
					}

					RefReader<byte> reader = new(buffer);
					foreach (GJsonType type in Enum.GetValues(typeof(GJsonType))) {
						for (var j = i; j < i + 10; j++) {
							var header = reader.Read();
							var rtype  = (GJsonType) (header & 0b00000111);
							var rj     = GJsonBinary.ReadVarLength(ref reader, header);
							Assert.Equal(type, rtype);
							Assert.Equal(j,    rj);
						}
					}
				}
			}
		}

		[Fact]
		public void VarLongTest() {
			(long min, long max)[] list = {
				new(sbyte.MinValue - 100, sbyte.MinValue + 100),
				new(sbyte.MaxValue - 100, sbyte.MaxValue + 100),

				new(byte.MinValue - 100, byte.MinValue + 100),
				new(byte.MaxValue - 100, byte.MaxValue + 100),

				new(short.MinValue - 100, short.MinValue + 100),
				new(short.MaxValue - 100, short.MaxValue + 100),

				new(ushort.MinValue - 100, ushort.MinValue + 100),
				new(ushort.MaxValue - 100, ushort.MaxValue + 100),

				new((long) int.MinValue - 100, int.MinValue        + 100),
				new(int.MaxValue        - 100, (long) int.MaxValue + 100),

				new(long.MinValue - 0, long.MinValue   + 100),
				new(long.MaxValue - 100, long.MaxValue + 0),
			};

			Span<byte> buffer = stackalloc byte[1024];

			//Uint32测试
			foreach (var (min, max) in list) {
				for (var i = min; i < max; i += 10) {
					RefWriter<byte> writer = new(buffer);
					foreach (GJsonType type in Enum.GetValues(typeof(GJsonType))) {
						for (var j = i; j < i + 10; j++) {
							GJsonBinary.WriteVarLong(ref writer, type, j);
						}
					}

					RefReader<byte> reader = new(buffer);
					foreach (GJsonType type in Enum.GetValues(typeof(GJsonType))) {
						for (var j = i; j < i + 10; j++) {
							var header = reader.Read();
							var rtype  = (GJsonType) (header & 0b00000111);
							var rj     = GJsonBinary.ReadVarLong(ref reader, header >> 3);
							Assert.Equal(type, rtype);
							Assert.Equal(j,    rj);
						}
					}
				}
			}
		}
	}
}