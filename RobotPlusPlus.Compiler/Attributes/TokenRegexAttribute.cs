using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace RobotPlusPlus.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class TokenRegexAttribute : Attribute
	{
		public string pattern;
		public RegexOptions options;

		public TokenRegexAttribute([RegexPattern, NotNull] string pattern, RegexOptions options = RegexOptions.IgnoreCase)
		{
			this.pattern = pattern;
			this.options = options;
		}

		public bool Evaluate([NotNull] string input)
		{
			return Regex.IsMatch(input, $@"^{pattern}$", options);
		}
	}
}