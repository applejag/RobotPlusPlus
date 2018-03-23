using System;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Separators and pairing characters. Ex: }, (, ;</summary>
	public class PunctuatorToken : Token
	{
		private static readonly Bictionary<char, char> parentasesPairs = new Bictionary<char, char>
		{
			{ '(', ')' },
			{ '[', ']' },
			{ '{', '}' },
		};

		private static readonly IReadOnlyCollection<char> separators = new []
		{
			';', ',',
		};

		public char Character { get; }
		public Type PunctuatorType { get; }

		public PunctuatorToken(TokenSource source) : base(source)
		{
			
			Character = SourceCode[0];

			if (parentasesPairs.ContainsKey(Character))
				PunctuatorType = Type.OpeningParentases;
			else if (parentasesPairs.ContainsValue(Character))
				PunctuatorType = Type.ClosingParentases;
			else if (separators.Contains(Character))
				PunctuatorType = Type.Separator;
			else
				throw new ParseUnexpectedTokenException(this);
		}
		

		public override void ParseToken(IteratedList<Token> parent)
		{
			switch (PunctuatorType)
			{
				case Type.OpeningParentases:
					CollectUntilClosingParentases(parent);
					break;

				case Type.Separator:
					break;
					
				// Closing parentases should be caught by the opening ones, otherwise they're stray
				default:
					throw new ParseUnexpectedTokenException(this);
			}
		}

		private void CollectUntilClosingParentases(IteratedList<Token> parent)
		{
			// Collect all until group found
			while (true)
			{
				Token next = parent.Next;
				switch (next)
				{
					case null:
						throw new ParseTokenException($"Unexpected EOF, expected <{GetMatchingParentases(Character)}>!", this);

					case PunctuatorToken open when open.PunctuatorType == Type.OpeningParentases:
						parent.ParseNextToken();
						goto default;

					case PunctuatorToken close when close.PunctuatorType == Type.ClosingParentases
						&& close.Character == GetMatchingParentases(Character):
						parent.PopNext();
						return; // stops

					default:
						this.Add(parent.PopNext());
						break;
				}
			}
		}

		public override string CompileToken(Compiler compiler)
		{
			switch (Character)
			{
				case '(':
				case ')':
				case '}':
				case ']':
					throw new ParseUnexpectedTokenException(this);

				case '{':
					var rows = new List<string>(this.Count);
					foreach (Token token in this)
					{
						compiler.assignmentNeedsCSSnipper = false;

						rows.Add(token.CompileToken(compiler));
					}

					return string.Join('\n', rows.Where(s => !string.IsNullOrEmpty(s)));
					
				case ';':
					return string.Empty;

				default:
					throw new NotImplementedException();
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
			Separator,
		}
	}
}