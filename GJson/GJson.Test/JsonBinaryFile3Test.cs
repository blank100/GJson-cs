using System.IO;

using Gal.Core;
using Gal.Core.GJson;

using Xunit;

public class JsonBinaryFile3Test {
	[Fact]
	public void File3Test() {
		var jsonString = File.ReadAllText("Resources/basic-huge.json");
		var json       = GJsonDecoder.Exec(jsonString);
			
		RefWriter<byte> writer = new(jsonString.Length);
			
		GJsonBinary.Encode(json, ref writer);
			
		var reader = new RefReader<byte>(writer.writtenSpan);
		var             rjson  = GJsonBinary.Decode(ref reader);
			
		Assert.Equal(json, rjson);
	}
}