using System;
using System.Linq;
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

		public static (TokenType type, int length) EvaluateType(string input, int line)
		{
			var values = (TokenType[]) Enum.GetValues(typeof(TokenType));

			foreach (TokenType type in values)
			{
				ITokenFilter[] attributes = type.GetEnumAttributes<ITokenFilter>();

				if (attributes.Length == 0)
					throw new ParseException($"Missing token filter for \"{type.ToString()}\".", line);
				
				// Lowest matching length amoung the filters
				int length = attributes.Min(f => f.MatchingLength(input));
				if (length > 0)
				{
					return (type: type, length: length);
				}
			}
			
			string inputPreview = input.Length > 24 ? input.Substring(0, 32) + "..." : input;
			throw new ParseException($"Unable to evaluate token type on segment \"{inputPreview}\".", line);
		}
	}
}