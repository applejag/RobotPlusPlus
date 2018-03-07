using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RobotPlusPlus.Tokenizing
{
	public static class Tokenizer
	{
		public static Token[] Tokenize(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
				return new Token[0];

			var tokens = new List<Token>();

			foreach (Match match in Regex.Matches(code, @"\S+"))
			{
				TokenType type = Token.EvaluateType(match.Value, 0);
				tokens.Add(new Token(type, match.Value));
			}

			return tokens.ToArray();
		}

	}
}