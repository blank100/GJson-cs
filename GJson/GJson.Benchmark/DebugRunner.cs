using System;
using System.IO;
using Gal.Core.GJson;
using LitJson;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Json.Benchmark
{
    public static class DebugRunner
    {
        public static void Run() {
            // var jsonString = File.ReadAllText("Resources/basic.json");
            // var json       = GJsonDecoder.ExecUnsafe(jsonString);
            // Assert.Equal(jsonString, json.ToString());
            
            // const string JSON1 = "Resources/Json01.txt";
            // const string JSON2 = "Resources/Json02.txt";
            //
            // var path1 = Path.Combine(Environment.CurrentDirectory, JSON1);
            // var path2 = Path.Combine(Environment.CurrentDirectory, JSON2);
            //
            // var m_Json1String = File.ReadAllText(path1);
            // var m_Json2String = File.ReadAllText(path2);
            //
            // var gdata1 = GJsonObject.Decode(m_Json1String);
            // var gdata2 = GJsonObject.Decode(m_Json2String);
            //
            // Console.WriteLine("GJson:" + gdata1.ToString(true));
            // Console.WriteLine("GJson:" + gdata2.ToString(true));
            //
            // var data1 = JsonMapper.ToObject(m_Json1String);
            // var data2 = JsonMapper.ToObject(m_Json2String);
            //
            // Console.WriteLine("litJson:" + data1.ToJson());
            // Console.WriteLine("litJson:" + data2.ToJson());
            //
            // var m_JObject1 = JObject.Parse(m_Json1String);
            // var m_JObject2 = JArray.Parse(m_Json2String);
            //
            // Console.WriteLine("newtonsoft:" + m_JObject1);
            // Console.WriteLine("newtonsoft:" + m_JObject2);
            
            // string jsonString = "{//this is comment\r\n\t\"name\": \"张\\\"三\",\r\n\t\"age\": 25, \r\n\t\"city\": \"北京\"\r\n,\"isBool\":true,\"isFalse\":false,\"isNull\":null}";
            // // var t = JObject.Parse(jsonString);
            // // var t = JsonMapper.ToObject(jsonString);
            // var t = GJsonDecoder.ExecUnsafe(jsonString);
            // var s = t.ToString();
        }
    }
}