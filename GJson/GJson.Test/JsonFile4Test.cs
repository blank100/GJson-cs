using System.IO;

using Gal.Core.GJson;

using Xunit;

namespace GalCoreUnitTest.Classes {
	public class JsonFile4Test {
		[Fact]
		public void DecodeFile4_Text() {
			var jsonString = File.ReadAllText("Resources/smart-quotes.json");
			var json       = GJsonDecoder.Exec(jsonString);
			Assert.Equal("{\"name\":\"Reference “Resetting”\"}", json.ToString());
		}

		[Fact]
		public void DecodeFile4_Unsafe() {
			var jsonString = File.ReadAllText("Resources/smart-quotes.json");
			var json       = GJsonDecoder.ExecUnsafe(jsonString);
			Assert.Equal("{\"name\":\"Reference “Resetting”\"}", json.ToString());
		}
	}
}