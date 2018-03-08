using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class TokenRegexAttribute : Attribute, ITokenFilter
	{
		public string pattern;
		public RegexOptions options;

		public TokenRegexAttribute([RegexPattern, NotNull] string pattern, RegexOptions options = RegexOptions.IgnoreCase)
		{
			this.pattern = pattern;
			this.options = options;
		}

		public bool Evaluate(string input)
		{
			return Regex.IsMatch(input, $@"^{pattern}$", options);
		}
	}
}