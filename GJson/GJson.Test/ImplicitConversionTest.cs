using Gal.Core;
using Gal.Core.GJson;

using Xunit;

namespace Json.Test {
	/// <summary>
	/// 
	/// </summary>
	/// <para>@author gouanlin</para>
	public class ImplicitConversionTest {
		[Fact]
		public static void NullString() {
			string      text = null;
			GJsonObject json = text;
			Assert.True(json.isNull);
			Assert.Equal("null", json.ToString());
		}

		[Fact]
		public static void ChildIsNullString() {
			string      text = null;
			GJsonObject json = new();
			json["child"] = text;
			Assert.True(json["child"].isNull);
			Assert.Equal("{\"child\":null}", json.ToString());
		}

		[Fact]
		public static void ChildIsNull() {
			GJsonObject json = new();

			json["child"] = null;
			Assert.False(json.HasChild("child"));
			Assert.Equal(0, json.count);

			json["child"] = "this is child";
			Assert.True(json.HasChild("child"));
			Assert.Equal(1, json.count);

			json["child"] = null; //删除 child 节点
			Assert.False(json.HasChild("child"));
			Assert.Equal(0, json.count);
		}

		[Fact]
		public static void ChildIsNumber() {
			GJsonObject json = new();

			json["child"] = 1;
			Assert.Equal(1, json["child"]);

			json["child"] = 1.5f;
			//此处不能隐式转换
			Assert.True(Number.Equals(1.5f, (float)json["child"]));

			json["child"] = (short) 35;
			Assert.Equal<short>(35, (short) json["child"]);

			json["child"] = 5681153L;
			Assert.Equal<long>(5681153L, json["child"]);

			json["child"] = 1.5d;
			Assert.True(Number.Equals(1.5d, json["child"]));
		}

		[Fact]
		public static void ChildIsBoolean() {
			GJsonObject json = new();

			json["child"] = true;
			Assert.True(json["child"]);

			json["child"] = false;
			Assert.False(json["child"]);
		}

		[Fact]
		public static void PrimalityBaseConversion() {
			//bool
			{
				GJsonObject json = false;
				Assert.Equal(GJsonType.Boolean, json.type);
				Assert.True(json.isBoolean);

				bool v = json;
				Assert.False(v);
			}

			//int
			{
				GJsonObject json = 1;
				Assert.True(json.isNumber);

				var v = (int) json;
				Assert.Equal(1, v);
			}
			
			//long
			{
				GJsonObject json = 100L;
				Assert.True(json.isNumber);
				Assert.Equal<long>(100L, json);

				long v = json;
				Assert.Equal(100L, v);
			}
			
			//double
			{
				GJsonObject json = 100.0D;
				Assert.True(json.isNumber);
				Assert.Equal<double>(100D, json);

				double v = json;
				Assert.Equal(100.0D,v);
			}
			
			//string
			{
				GJsonObject json = "this is text";
				Assert.True(json.isString);
				Assert.Equal("this is text",json);

				string v = json;
				Assert.Equal("this is text", v);
			}
		}
	}
}