namespace Gal.Core
{
	/// <summary>
	/// 
	/// </summary>
	/// <para>author gouanlin</para>
	public static class StringToNumber
	{
		public static int StringToInt(string text) {
			int n = 0, l = text.Length;
			for (var i = 0; i < l; i++) n = n * 10 + (text[i] - '0');
			return n;
		}

		public static long StringToLong(string text) {
			var n = 0L;
			var l = text.Length;
			for (var i = 0; i < l; i++) n = n * 10L + (text[i] - '0');
			return n;
		}
	}
}