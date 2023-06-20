using System.IO;

using Gal.Core;
using Gal.Core.GJson;

using Xunit;

public class JsonBinaryFile2Test {
	[Fact]
	public void File2Test() {
		var jsonString = File.ReadAllText("Resources/basic.json");
		var json       = GJsonObject.Decode(jsonString);
			
		RefWriter<byte> writer = new(jsonString.Length);
			
		GJsonBinary.Encode(json, ref writer);
			
		var reader = new RefReader<byte>(writer.writtenSpan);
		var             rjson  = GJsonBinary.Decode(ref reader);
			
		Assert.Equal(json, rjson);
	}
}