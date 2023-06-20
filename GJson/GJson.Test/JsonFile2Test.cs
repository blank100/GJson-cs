using System.IO;

using Gal.Core.GJson;

using Xunit;

namespace GalCoreUnitTest.Classes {
	public class JsonFile2Test {
		[Fact]
		public void DecodeFile2_Text() {
			var jsonString = File.ReadAllText("Resources/basic.json");
			var json       = GJsonObject.Decode(jsonString);
			Assert.Equal(jsonString, json.ToString());
		}

		// [Fact]
		// public void DecodeFile2_Unsafe() {
		// 	var jsonString = File.ReadAllText("Resources/basic.json");
		// 	var json       = GJsonDecoder.ExecUnsafe(jsonString);
		// 	Assert.Equal(jsonString, json.ToString());
		// }
	}
}