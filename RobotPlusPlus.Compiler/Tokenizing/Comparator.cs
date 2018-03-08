using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace RobotPlusPlus.Tokenizing
{
	public static class Comparator
	{

		public static int MatchingRegex([NotNull] string input, [RegexPattern, NotNull] string pattern, RegexOptions options = RegexOptions.IgnoreCase)
		{
			return Regex.Match(input, $@"^{pattern}", options).Length;
		}
	}
}