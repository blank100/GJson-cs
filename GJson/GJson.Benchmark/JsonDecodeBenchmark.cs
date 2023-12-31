using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Gal.Core;
using Gal.Core.GJson;
using LitJson;
using MessagePack;
using Newtonsoft.Json.Linq;

namespace Json.Benchmark
{
    [MemoryDiagnoser]
    public class JsonDecodeBenchmark
    {
        private const int COUNT = 100;

        private const string JSON1 = "Resources/Json01.txt";
        private const string JSON2 = "Resources/Json02.txt";
        private const string JSON3 = "Resources/basic.json";

        private string m_Json1String;
        private string m_Json2String;
        private string m_Json3String;

        private GJsonObject m_Json1;
        private GJsonObject m_Json2;
        private GJsonObject m_Json3;

        private JsonData m_JsonData1;
        private JsonData m_JsonData2;
        private JsonData m_JsonData3;

        private JObject m_JObject1;
        private JArray m_JObject2;
        private JObject m_JObject3;

        private byte[] m_MspBytes1;
        private byte[] m_MspBytes2;

        [GlobalSetup]
        public void Setup() {
            var path1 = Path.Combine(Environment.CurrentDirectory, JSON1);
            var path2 = Path.Combine(Environment.CurrentDirectory, JSON2);
            var path3=Path.Combine(Environment.CurrentDirectory,   JSON3);

            m_Json1String = File.ReadAllText(path1);
            m_Json2String = File.ReadAllText(path2);
            m_Json3String = File.ReadAllText(path3);

            m_Json1 = GJsonObject.Decode(m_Json1String);
            m_Json2 = GJsonObject.Decode(m_Json2String);
            m_Json3 = GJsonObject.Decode(m_Json3String);

            m_JsonData1 = JsonMapper.ToObject(m_Json1String);
            m_JsonData2 = JsonMapper.ToObject(m_Json2String);
            m_JsonData3 = JsonMapper.ToObject(m_Json3String);

            m_JObject1 = JObject.Parse(m_Json1String);
            m_JObject2 = JArray.Parse(m_Json2String);
            m_JObject3 = JObject.Parse(m_Json3String);
        }

        [Benchmark(Baseline = true)]
        public void GJson_ToJsonObject() {
            for (var i = 0; i < COUNT; i++) {
                using var data1 = GJsonObject.Decode(m_Json1String);
                using var data2 = GJsonObject.Decode(m_Json2String);
                using var data3 = GJsonObject.Decode(m_Json3String);
            }
        }

        [Benchmark()]
        public void LitJson_ToJsonData() {
            for (var i = 0; i < COUNT; i++) {
                var data1 = JsonMapper.ToObject(m_Json1String);
                var data2 = JsonMapper.ToObject(m_Json2String);
                var data3 = JsonMapper.ToObject(m_Json3String);
            }
        }

        [Benchmark()]
        public void NewtonSoft_ToJObject() {
            for (var i = 0; i < COUNT; i++) {
                var data1 = JObject.Parse(m_Json1String);
                var data2 = JArray.Parse(m_Json2String);
                var data3 = JObject.Parse(m_Json3String);
            }
        }

        [Benchmark()]
        public void GJson_ToString() {
            for (var i = 0; i < COUNT; i++) {
                m_Json1.ToJsonString();
                m_Json2.ToJsonString();
                m_Json3.ToJsonString();
            }
        }

        [Benchmark()]
        public void LitJson_ToString() {
            for (var i = 0; i < COUNT; i++) {
                m_JsonData1.ToJson();
                m_JsonData2.ToJson();
                m_JsonData3.ToJson();
            }
        }

        [Benchmark()]
        public void NewtonSoft_ToString() {
            for (var i = 0; i < COUNT; i++) {
                m_JObject1.ToString();
                m_JObject2.ToString();
                m_JObject3.ToString();
            }
        }

        [Benchmark()]
        public void GJson_ToJsonObjectAndToString() {
            for (var i = 0; i < COUNT; i++) {
                using var data1 = GJsonObject.Decode(m_Json1String);
                using var data2 = GJsonObject.Decode(m_Json2String);
                using var data3 = GJsonObject.Decode(m_Json3String);
                data1.ToString();
                data2.ToString();
                data3.ToString();
            }
        }

        [Benchmark()]
        public void LitJson_ToJsonDataAndToString() {
            for (var i = 0; i < COUNT; i++) {
                var data1 = JsonMapper.ToObject(m_Json1String);
                var data2 = JsonMapper.ToObject(m_Json2String);
                var data3 = JsonMapper.ToObject(m_Json3String);
                data1.ToJson();
                data2.ToJson();
                data3.ToJson();
            }
        }

        // [Benchmark()]
        // public unsafe void EncodeBinary() {
        //     for (var i = 0; i < COUNT; i++) {
        //         RefWriter<byte> writer1 = new(stackalloc byte[256]);
        //         GJsonBinary.Encode(m_Json1, ref writer1);
        //
        //         RefWriter<byte> writer2 = new(stackalloc byte[256]);
        //         GJsonBinary.Encode(m_Json2, ref writer2);
        //     }
        // }
        //
        // [Benchmark()]
        // public void DecodeString() {
        //     for (var i = 0; i < COUNT; i++) {
        //         using var json1 = GJsonObject.Decode(m_Json1String);
        //         using var json2 = GJsonObject.Decode(m_Json2String);
        //     }
        // }
        //
        // [Benchmark()]
        // public void DecodeStringUnsafe() {
        //     for (var i = 0; i < COUNT; i++) {
        //         using var json1 = GJsonDecoder.ExecUnsafe(m_Json1String);
        //         using var json2 = GJsonDecoder.ExecUnsafe(m_Json2String);
        //     }
        // }
        //
        // [Benchmark()]
        // public void DecodeMessagepack_Bytes() {
        //     for (var i = 0; i < COUNT; i++) {
        //         MessagePackSerializer.ConvertToJson(m_MspBytes1);
        //         MessagePackSerializer.ConvertToJson(m_MspBytes2);
        //     }
        // }
        //
        // [Benchmark()]
        // public void DecodeMessagepack_String() {
        //     for (var i = 0; i < COUNT; i++) {
        //         MessagePackSerializer.ConvertFromJson(m_Json1String);
        //         MessagePackSerializer.ConvertFromJson(m_Json2String);
        //     }
        // }
        //
        // [Benchmark()]
        // public void DecodeBinary() {
        //     for (var i = 0; i < COUNT; i++) {
        //         RefReader<byte> reader1 = new(m_JsonBytes1);
        //         using var json1 = GJsonBinary.Decode(ref reader1);
        //         var s1 = json1.ToString();
        //
        //         RefReader<byte> reader2 = new(m_JsonBytes2);
        //         using var json2 = GJsonBinary.Decode(ref reader2);
        //         var s2 = json2.ToString();
        //     }
        // }

        // [Benchmark]
        // public void DecodeStringUnsafe() {
        // 	for (var i = 0; i < COUNT; i++) {
        // 		using var json1 = GJsonDecoder.ExecUnsafe(m_Json1String);
        // 		using var json2 = GJsonDecoder.ExecUnsafe(m_Json2String);
        // 	}
        // }
    }
}