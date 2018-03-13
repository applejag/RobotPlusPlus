using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Separators and pairing characters. Ex: }, (, ;</summary>
	public class Punctuator : Token
	{
		private static readonly Bictionary<char, char> parentasesPairs = new Bictionary<char, char>
		{
			{ '(', ')' },
			{ '[', ']' },
			{ '{', '}' },
		};

		public char Character { get; }
		public Type PunctuatorType { get; }

		public Punctuator(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			Character = sourceCode[0];

			if (parentasesPairs.ContainsKey(Character))
				PunctuatorType = Type.OpeningParentases;
			else if (parentasesPairs.ContainsValue(Character))
				PunctuatorType = Type.ClosingParentases;
			else
				PunctuatorType = Type.Other;
		}

		public override void ParseToken(Parser parser)
		{
			switch (PunctuatorType)
			{
				case Type.OpeningParentases:
					// Collect all until group found
					while (true)
					{
						if (parser.NextToken == null)
							throw new ParseException($"Unexpected EOF, expected <{GetMatchingParentases(Character)}>!", parser.CurrToken);

						Token nextToken = parser.TakeNextToken(Count);

						// Stop when found matching pair
						if (nextToken is Punctuator punc
							&& punc.PunctuatorType == Type.ClosingParentases
							&& punc.Character == GetMatchingParentases(Character))
							break;
					}
					break;

				case Type.Other when Character == '.' && parser.NextToken is Identifier:
					break;

				case Type.ClosingParentases:
					bool someoneLookingForMe = parser.IsTokenParsing<Punctuator>(p =>
						p.PunctuatorType == Type.OpeningParentases
						&& p.Character == GetMatchingParentases(Character));
					if (!someoneLookingForMe)
						throw new ParseException($"Unexpected ending parentases <{SourceCode}>.", this);
					break;

				default:
					throw new ParseException($"Unexpected punctuator <{SourceCode}>.", this);
			}
		}

		public static char GetMatchingParentases(char c)
		{
			return parentasesPairs[c];
		}

		public enum Type
		{
			OpeningParentases,
			ClosingParentases,
			Other,
		}
	}
}