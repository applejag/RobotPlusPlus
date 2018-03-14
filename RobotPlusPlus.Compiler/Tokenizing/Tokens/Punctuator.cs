using System;
using System.Collections.Generic;
using System.Linq;
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

		private static readonly IReadOnlyCollection<char> separators = new []
		{
			';', ',', ':'
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
			else if (separators.Contains(Character))
				PunctuatorType = Type.Separator;
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

						Token takenToken = parser.TakeNextToken();

						// Stop when found matching pair
						if (takenToken is Punctuator punc
						    && punc.PunctuatorType == Type.ClosingParentases
						    && punc.Character == GetMatchingParentases(Character))
						{
							break;
						}
					}
					break;

				case Type.Separator:
					break;

				case Type.Other when Character == '.' && parser.NextToken is Identifier:
					if (TrailingWhitespace != null)
						throw new ParseException(
							$"Unexpected whitespace after punctuator <{Character}> before identifier <{parser.NextToken.SourceCode}>.",
							this);
					else
						parser.TakeNextToken();
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

		public override string CompileToken()
		{
			throw new NotImplementedException();
		}

		public static char GetMatchingParentases(char c)
		{
			return parentasesPairs[c];
		}

		public enum Type
		{
			OpeningParentases,
			ClosingParentases,
			Separator,
			Other,
		}
	}
}