using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Separators and pairing characters. Ex: }, (, ;</summary>
	public class Punctuator : Token
	{
		private static readonly Dictionary<char, char> parentasesPairs = new Dictionary<char, char>
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
							throw new ParseException($"Unexpected EOF, expected <{parentasesPairs[Character]}>!", parser.CurrToken);

						Token nextToken = parser.TakeNextToken();
						Tokens.Add(nextToken);

						// Stop when found matching pair
						if (nextToken is Punctuator punc
						    && punc.PunctuatorType == Type.ClosingParentases
						    && punc.Character == parentasesPairs[Character])
							break;
					}
					break;

				case Type.Other when Character == '.' && parser.NextToken is Identifier:
					break;

				default:
					throw new ParseException($"Unexpected punctuator <{SourceCode}>.", this);
			}
		}

		public enum Type
		{
			OpeningParentases,
			ClosingParentases,
			Other,
		}
	}
}