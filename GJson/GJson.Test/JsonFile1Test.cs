using System.IO;

using Gal.Core.GJson;

using Xunit;

namespace GalCoreUnitTest.Classes {
	public class JsonFile1Test {
		[Fact]
		public void DecodeFile1_Text() {
			var jsonString = File.ReadAllText("Resources/allow-newlines-inside-strings.json");
			var json       = GJsonObject.Decode(jsonString);
			Assert.Equal(jsonString, json.ToString());
		}

		// [Fact]
		// public void DecodeFile1_Unsafe() {
		// 	var jsonString = File.ReadAllText("Resources/allow-newlines-inside-strings.json");
		// 	var json       = GJsonDecoder.ExecUnsafe(jsonString);
		// 	Assert.Equal(jsonString, json.ToString());
		// }
	}
}