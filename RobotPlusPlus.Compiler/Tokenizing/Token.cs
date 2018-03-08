using System;
using System.Linq;
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
				ITokenFilter[] attributes = type.GetEnumAttributes<ITokenFilter>();

				if (attributes.Length == 0)
					throw new ParseException($"Missing token filter for \"{type.ToString()}\".", line);
				
				if (attributes.All(f => f.Evaluate(input)))
					return type;
			}

			throw new ParseException("Unable to evaluate token type.", line);
		}
	}
}