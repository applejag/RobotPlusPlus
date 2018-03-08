using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class TokenWordInListAttribute : Attribute, ITokenFilter
	{
		public string[] Words { get; set; }

		/// <summary>Default: false</summary>
		public bool IgnoreCase { get; set; } = false;

		public TokenWordInListAttribute([NotNull, ItemNotNull] params string[] words)
		{
			Words = words;
		}

		public int MatchingLength(string input)
		{
			if (Words == null) return 0;

			string match = Regex.Match(input, @"^\w*").Value;
			StringComparison option = IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

			foreach (string word in Words)
			{
				if (match.Equals(word, option))
				{
					return word.Length;
				}
			}

			return 0;
		}
	}
}