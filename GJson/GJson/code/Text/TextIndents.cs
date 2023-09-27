using System.Collections.Generic;

namespace Gal.Core
{
	/// <summary>
	/// 缩进符
	/// </summary>
	/// <para>author gouanlin</para>
	public static class TextIndents
	{
		private static readonly List<string> m_Indents = new() {
			"",
			"\t",
			"\t\t",
			"\t\t\t",
			"\t\t\t\t",
			"\t\t\t\t\t",
			"\t\t\t\t\t\t",
			"\t\t\t\t\t\t\t",
			"\t\t\t\t\t\t\t\t"
		};

		public static string GetIndent(int level) {
			var indents = m_Indents;

			var l = indents.Count;
			if (level < l) return indents[level];

			var t = indents[l - 1];
			for (var i = l - 1; i < level; i++) {
				t += "\t";
				indents.Add(t);
			}
			return indents[level];
		}
	}
}