using System.Collections.Generic;

using Gal.Core.GJson;

using Xunit;

namespace GalCoreUnitTest.Classes {
	public class JsonTest {
		public void NullTest() {
			GJsonObject data = new();
			var         getNullValue = data["key"];
			data["key"] = null;
			
			Assert.Equal(null,getNullValue);
			Assert.Equal(null,data["key"]);
		}
		
		[Fact]
		public void AsArrayTest() {
			GJsonObject data = new();
			data.Add(1);
			data.Add(2);
			data.Add(3);
			data.Add("Launch!");

			Assert.True(data.isArray);
			Assert.Equal("[1,2,3,\"Launch!\"]", data.ToString());
		}

		[Fact]
		public void AsBooleanTest() {
			GJsonObject data = true;
			Assert.True(data.isBoolean);
			Assert.True((bool) data);
			Assert.Equal("true", data.ToString());

			data = false;
			var f = false;

			Assert.Equal(f, (bool) data);
		}

		[Fact]
		public void AsDoubleTest() {
			GJsonObject data = 3e6;
			Assert.True(data.isNumber);
			Assert.Equal(3e6,       (double) data);
			Assert.Equal("3000000", data.ToString());

			data = 3.14;
			Assert.True(data.isNumber);
			Assert.Equal(3.14,   (double) data);
			Assert.Equal("3.14", data.ToString());

			data = 0.123;
			var n = 0.123;

			Assert.Equal(n, (double) data);
		}

		[Fact]
		public void AsIntTest() {
			GJsonObject data = 13;
			Assert.True(data.isNumber);
			Assert.Equal(13,   (int) data);
			Assert.Equal("13", data.ToString());

			data = -00500;

			Assert.True(data.isNumber);
			Assert.Equal(-500,   (int) data);
			Assert.Equal("-500", data.ToString());

			data = 1024;
			Assert.Equal(1024, (int) data);
		}

		[Fact]
		public void AsObjectTest() {
			GJsonObject data = new();

			data["alignment"]     = "left";
			data["font"]          = new();
			data["font"]["name"]  = "Arial";
			data["font"]["style"] = "italic";
			data["font"]["size"]  = 10;
			data["font"]["color"] = "#fff";

			Assert.True(data.isObject);

			const string json = "{\"alignment\":\"left\",\"font\":{" + "\"name\":\"Arial\",\"style\":\"italic\",\"size\":10," + "\"color\":\"#fff\"}}";

			Assert.Equal(json, data.ToString());
		}

		[Fact]
		public void AsStringTest() {
			GJsonObject data = "All you need is love";
			Assert.True(data.isString);
			Assert.Equal("All you need is love",     (string) data);
			Assert.Equal("\"All you need is love\"", data.ToString());
		}

		[Fact]
		public void EqualsTest() {
			GJsonObject a;
			GJsonObject b;

			// Compare ints
			a = 7;
			b = 7;
			Assert.True(a.Equals(b));

			Assert.False(a.Equals(null));

			b = 8;
			Assert.False(a.Equals(b));

			// Compare longs
			a = 10L;
			b = 10L;
			Assert.True(a.Equals(b));

			// Int now comparable to long
			b = 10;
			Assert.True(a.Equals(b));
			b = 11L;
			Assert.False(a.Equals(b));

			// Compare doubles
			a = 78.9;
			b = 78.9;
			Assert.True(a.Equals(b));

			b = 78.899999;
			Assert.False(a.Equals(b));

			b = 78.8999999;
			Assert.False(a.Equals(b));

			b = 78.900001;
			Assert.False(a.Equals(b));

			b = 78.9000001;
			Assert.False(a.Equals(b));

			// Compare booleans
			a = true;
			b = true;
			Assert.True(a.Equals(b));

			b = false;
			Assert.False(a.Equals(b));

			// Compare strings
			a = "walrus";
			b = "walrus";
			Assert.True(a.Equals(b));

			b = "Walrus";
			Assert.False(a.Equals(b));
		}

		[Fact]
		public void SystemObject2JsonTest1() {
			var o = new Dictionary<string, object>() {
				["stringKey"] = new List<object>() {
					"element1", 1, long.MaxValue, 3.14159f,
				},
				["key2"]=123456
			};
			var text = "{\"stringKey\":[\"element1\",1,9223372036854775807,3.14159],\"key2\":123456}";

			var json = SystemObjectToGJson.ToJson(o);
			Assert.Equal(text, json.ToString(false,"0.#####"));
			Assert.Equal(text,o.ToJsonString(false,"0.#####"));
		}
	}
}