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

		private static readonly IReadOnlyCollection<char> separators = new[]
		{
			';', ',',
		};

		public char Character { get; }
		public Type PunctuatorType { get; }

		public Token DotLHS
		{
			get => this[0];
			set => this[0] = value;
		}

		public IdentifierToken DotRHS
		{
			get => this[1] as IdentifierToken;
			set => this[1] = value;
		}

		public PunctuatorToken(TokenSource source) : base(source)
		{

			Character = SourceCode[0];

			if (parentasesPairs.ContainsKey(Character))
				PunctuatorType = Type.OpeningParentases;
			else if (parentasesPairs.ContainsValue(Character))
				PunctuatorType = Type.ClosingParentases;
			else if (separators.Contains(Character))
				PunctuatorType = Type.Separator;
			else if (Character == '.')
				PunctuatorType = Type.Dot;
			else
				throw new ParseUnexpectedTokenException(this);
		}


		public override void ParseToken(IteratedList<Token> parent)
		{
			switch (PunctuatorType)
			{
				case Type.OpeningParentases:
					CollectUntilClosingParentases(parent);

					if (Character == '(')
					{
						// Merge with identifier, this is a function call
						if (parent.Previous is IdentifierToken id1)
						{
							TokenSource tokenSource = id1.source;
							tokenSource.code += "()";
							parent.PopPrevious(); // id
							parent.PushPrevious(new FunctionCallToken(tokenSource, id1, this));
							parent.PopCurrent(); // parentases
							parent.ParseTokenAt(parent.Index);
						}
						else if (Count == 0)
							throw new ParseTokenException("Unexpected empty parentases group <()>.", this);
					}

					break;

				case Type.Separator:
					break;

				case Type.Dot:
					// Expect trailing identifier and no whitespace
					if (TrailingWhitespaceToken != null)
						throw new ParseUnexpectedTrailingTokenException(this, TrailingWhitespaceToken);
					if (!(parent.Next is IdentifierToken rhs))
						throw new ParseUnexpectedTrailingTokenException(this, parent.Next);

					Token lhs = parent.Previous;
					if (!(lhs is IdentifierToken
						|| (lhs is PunctuatorToken pun && pun.Character == '(')
						|| lhs is LiteralToken
						|| lhs is FunctionCallToken))
						throw new ParseUnexpectedLeadingTokenException(this, parent.Previous);

					DotLHS = lhs;
					DotRHS = rhs;
					parent.PopPrevious();
					parent.PopNext();
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

		public static char GetMatchingParentases(char c)
		{
			return parentasesPairs[c];
		}

		public enum Type
		{
			OpeningParentases,
			ClosingParentases,
			Separator,
			Dot,
		}
	}
}