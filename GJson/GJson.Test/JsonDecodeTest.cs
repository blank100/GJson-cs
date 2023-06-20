using System;
using System.IO;

using Gal.Core;
using Gal.Core.GJson;

using Xunit;

namespace Json.Test {
	public static class JsonDecodeTest {
		private const string JSON1 = "Resources/Json01.txt";
		private const string JSON2 = "Resources/Json02.txt";

		private const string IP_ADDRESS                 = "Resources/IPAddress.txt";
		private const string Headers                    = "Resources/Headers.txt";
		private const string Echo                       = "Resources/Echo.txt";
		private const string DateTime                   = "Resources/DateTime.txt";
		private const string Validation                 = "Resources/Validation.txt";
		private const string AllowNewLinesInsideStrings = "Resources/allow-newlines-inside-strings.json";
		private const string Basic                      = "Resources/basic.json";
		private const string BasicHuge                  = "Resources/basic-huge.json";
		private const string SmartQuotes                = "Resources/smart-quotes.json";

		[Fact]
		public static void NullTest() {
			var         text = "null";
			var json = GJsonObject.Decode(text);
			Assert.Equal(null,json);
		}
		
		[Fact]
		public static void BinaryTest() {
			GJsonObject json = new();
			json["key1"]         = "key1value";
			json["key2"]         = 15422;
			json["Array"]        = new();
			json["Array"][0]     = 1;
			json["Array"][1]     = "text";
			json["Array"][2]     = false;
			json["Array"][3]     = true;
			json["Array"][4]     = new();
			json["Array"][5]     = null;
			json["Array"][6]     = 3.1415926;
			json["Array2"]       = new();
			json["Array2"]["kk"] = 123456;
			json["Array2"]["a"]  = 123456;
			json["Array2"]["b"]  = 123456;
			json["Array2"]["c"]  = 123456;

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json,resultJson);
		}

		[Fact]
		public static void Decode1() {
			var path       = Path.Combine(Environment.CurrentDirectory, JSON1);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void Decode2() {
			var path       = Path.Combine(Environment.CurrentDirectory, JSON2);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeIpAddress() {
			var path       = Path.Combine(Environment.CurrentDirectory, IP_ADDRESS);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeHeaders() {
			var path       = Path.Combine(Environment.CurrentDirectory, Headers);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeEcho() {
			var path       = Path.Combine(Environment.CurrentDirectory, Echo);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeDateTime() {
			var path       = Path.Combine(Environment.CurrentDirectory, DateTime);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeValidation() {
			var path       = Path.Combine(Environment.CurrentDirectory, Validation);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeAllowNewLinesInsideStrings() {
			var path       = Path.Combine(Environment.CurrentDirectory, AllowNewLinesInsideStrings);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}

		[Fact]
		public static void DecodeBasic() {
			var path       = Path.Combine(Environment.CurrentDirectory, Basic);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}
		
		[Fact]
		public static void DecodeBasicHuge() {
			var path       = Path.Combine(Environment.CurrentDirectory, BasicHuge);
			var jsonString = File.ReadAllText(path);
			var json       = GJsonObject.Decode(jsonString);
			Console.Write(json.ToString(true));

			RefWriter<byte> writer = new(1024);
			GJsonBinary.Encode(json, ref writer);
			RefReader<byte> reader     = new(writer.writtenSpan);
			var             resultJson = GJsonBinary.Decode(ref reader);
			Assert.Equal(json, resultJson);
		}
		
		// [Fact]
		// public static void DecodeSmartQuotes() {
		// 	var path       = Path.Combine(Environment.CurrentDirectory, SmartQuotes);
		// 	var jsonString = File.ReadAllText(path);
		// 	var json       = GJsonDecoder.ExecUnsafe(jsonString);
		// 	Console.Write(json.ToString(true));
		//
		// 	RefWriter<byte> writer = new(1024);
		// 	GJsonBinary.Encode(json, ref writer);
		// 	RefReader<byte> reader     = new(writer.writtenSpan);
		// 	var             resultJson = GJsonBinary.Decode(ref reader);
		// 	Assert.Equal(json, resultJson);
		// }
	}
}