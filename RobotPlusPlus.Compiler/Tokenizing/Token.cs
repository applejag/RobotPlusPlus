using System;
using RobotPlusPlus.Attributes;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Tokenizing
{
	public class Token
	{
		public TokenType Type { get; }
		public string Source { get; }

		public Token(TokenType type, string source)
		{
			Type = type;
			Source = source;
		}

		public static TokenType EvaluateType(string input, int line)
		{
			var values = (TokenType[]) Enum.GetValues(typeof(TokenType));

			foreach (TokenType type in values)
			{
				var attribute = type.GetEnumAttribute<TokenRegexAttribute>();

				if (attribute == null)
					throw new ParseException($"Missing token pattern for \"{type.ToString()}\".", line);

				if (attribute.Evaluate(input))
					return type;
			}

			throw new ParseException("Unable to evaluate token type.", line);
		}
	}
}