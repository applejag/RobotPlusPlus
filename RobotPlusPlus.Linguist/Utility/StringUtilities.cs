using System.Text;
using JetBrains.Annotations;

namespace RobotPlusPlus.Linguist.Utility
{
	public static class StringUtilities
	{
		private static readonly Bictionary<char, string> escapedCharacters = new Bictionary<char, string>
		{
			{ '\a', @"\a" },
			{ '\b', @"\b" },
			{ '\f', @"\f" },
			{ '\n', @"\n" },
			{ '\r', @"\r" },
			{ '\t', @"\t" },
			{ '\v', @"\v" },
			{ '"', @"\""" },
			{ '\\', @"\\" },
		};

		public static string EscapeString([NotNull] this string text)
		{
			var escaped = new StringBuilder();

			foreach (char c in text)
			{
				if (escapedCharacters.TryGetValue(c, out string e))
					escaped.Append(e);
				else
					escaped.Append(c);
			}

			return escaped.ToString();
		}

		public static bool EndsWith([NotNull] this string value, char c, bool ignoreCase)
		{
			if (value.Length == 0)
				return false;

			if (ignoreCase)
				return char.ToUpperInvariant(value[value.Length - 1]) == char.ToUpperInvariant(c);

			return value[value.Length - 1] == c;
		}
	}
}