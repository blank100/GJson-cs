using System;
using System.IO;

using BenchmarkDotNet.Attributes;

using Gal.Core;
using Gal.Core.GJson;

using MessagePack;

namespace Json.Benchmark {
	[MemoryDiagnoser]
	public class JsonDecodeBenchmark {
		private const int COUNT = 100;

		private const string JSON1 = "Resources/Json01.txt";
		private const string JSON2 = "Resources/Json02.txt";

		private string m_Json1String;
		private string m_Json2String;

		private GJsonObject m_Json1;
		private GJsonObject m_Json2;

		private byte[] m_JsonBytes1;
		private byte[] m_JsonBytes2;

		private byte[] m_MspBytes1;
		private byte[] m_MspBytes2;

		[GlobalSetup]
		public void Setup() {
			var path1 = Path.Combine(Environment.CurrentDirectory, JSON1);
			var path2 = Path.Combine(Environment.CurrentDirectory, JSON2);

			m_Json1String = File.ReadAllText(path1);
			m_Json2String = File.ReadAllText(path2);

			m_Json1 = GJsonDecoder.Exec(m_Json1String);
			m_Json2 = GJsonDecoder.Exec(m_Json2String);

			RefWriter<byte> writer1 = new(stackalloc byte[256]);
			GJsonBinary.Encode(m_Json1, ref writer1);
			var span1 = writer1.writtenSpan;
			m_JsonBytes1 = new byte[span1.Length];
			span1.CopyTo(m_JsonBytes1);

			RefWriter<byte> writer2 = new(stackalloc byte[256]);
			GJsonBinary.Encode(m_Json2, ref writer2);
			var span2 = writer1.writtenSpan;
			m_JsonBytes2 = new byte[span2.Length];
			span2.CopyTo(m_JsonBytes2);

			m_MspBytes1 = MessagePackSerializer.ConvertFromJson(m_Json1String);
			m_MspBytes2 = MessagePackSerializer.ConvertFromJson(m_Json1String);
		}

		[Benchmark(Baseline = true)]
		public void EncodeString() {
			for (var i = 0; i < COUNT; i++) {
				var text1 = m_Json1.ToString();
				var text2 = m_Json2.ToString();
			}
		}

		[Benchmark()]
		public unsafe void EncodeBinary() {
			for (var i = 0; i < COUNT; i++) {
				RefWriter<byte> writer1 = new(stackalloc byte[256]);
				GJsonBinary.Encode(m_Json1, ref writer1);

				RefWriter<byte> writer2 = new(stackalloc byte[256]);
				GJsonBinary.Encode(m_Json2, ref writer2);
			}
		}

		[Benchmark()]
		public void DecodeString() {
			for (var i = 0; i < COUNT; i++) {
				using var json1 = GJsonDecoder.Exec(m_Json1String);
				using var json2 = GJsonDecoder.Exec(m_Json2String);
			}
		}

		[Benchmark()]
		public void DecodeStringUnsafe() {
			for (var i = 0; i < COUNT; i++) {
				using var json1 = GJsonDecoder.ExecUnsafe(m_Json1String);
				using var json2 = GJsonDecoder.ExecUnsafe(m_Json2String);
			}
		}

		[Benchmark()]
		public void DecodeMessagepack_Bytes() {
			for (var i = 0; i < COUNT; i++) {
				MessagePackSerializer.ConvertToJson(m_MspBytes1);
				MessagePackSerializer.ConvertToJson(m_MspBytes2);
			}
		}

		[Benchmark()]
		public void DecodeMessagepack_String() {
			for (var i = 0; i < COUNT; i++) {
				MessagePackSerializer.ConvertFromJson(m_Json1String);
				MessagePackSerializer.ConvertFromJson(m_Json2String);
			}
		}

		[Benchmark()]
		public void DecodeBinary() {
			for (var i = 0; i < COUNT; i++) {
				RefReader<byte> reader1 = new(m_JsonBytes1);
				using var       json1   = GJsonBinary.Decode(ref reader1);
				var             s1      = json1.ToString();

				RefReader<byte> reader2 = new(m_JsonBytes2);
				using var       json2   = GJsonBinary.Decode(ref reader2);
				var             s2      = json2.ToString();
			}
		}

		// [Benchmark]
		// public void DecodeStringUnsafe() {
		// 	for (var i = 0; i < COUNT; i++) {
		// 		using var json1 = GJsonDecoder.ExecUnsafe(m_Json1String);
		// 		using var json2 = GJsonDecoder.ExecUnsafe(m_Json2String);
		// 	}
		// }
	}
}