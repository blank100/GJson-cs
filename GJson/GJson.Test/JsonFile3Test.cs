using System.IO;

using Gal.Core.GJson;

using Xunit;

namespace GalCoreUnitTest.Classes {
	public class JsonFile3Test {
		[Fact]
		public void DecodeFile3_Text() {
			var jsonString = File.ReadAllText("Resources/basic-huge.json");
			var json       = GJsonObject.Decode(jsonString);
			Assert.Equal(jsonString, json.ToString());
		}

		// [Fact]
		// public void DecodeFile3_Unsafe() {
		// 	var jsonString = File.ReadAllText("Resources/basic-huge.json");
		// 	var json       = GJsonDecoder.ExecUnsafe(jsonString);
		// 	Assert.Equal(jsonString, json.ToString());
		// }
	}
}