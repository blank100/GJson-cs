using Gal.Core.GJson;

using Xunit;

namespace Json.Benchmark {
	public static class DebugRunner {
		public static void Run() {
			var         text = "null";
			var json = GJsonDecoder.Exec(text);
			Assert.Equal(null,json);
		}
	}
}