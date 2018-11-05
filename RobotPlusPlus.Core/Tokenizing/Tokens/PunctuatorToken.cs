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

		public IdentifierToken ColonName
		{
			get => this[0] as IdentifierToken;
			set => this[0] = value;
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
			else if (Character == ':')
				PunctuatorType = Type.Colon;
			else
				throw new ParseUnexpectedTokenException(this);
		}

		public override void ParseToken(IteratedList<Token> parent)
		{
			switch (PunctuatorType)
			{
				case Type.OpeningParentases:
					CollectUntilClosingParentases(parent);
					ConvertIntoFunctionCall(parent);
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
					      || IsPunctuatorOfType(lhs, Type.Dot)
					      || IsOpenParentasesOfChar(lhs, '(')
					      || lhs is LiteralToken
					      || lhs is FunctionCallToken))
						throw new ParseUnexpectedLeadingTokenException(this, parent.Previous);

					DotLHS = lhs;
					DotRHS = rhs;
					parent.PopPrevious();
					parent.PopNext();
					break;

				case Type.Colon:
					if (!(parent.Previous is IdentifierToken name))
						throw new ParseUnexpectedLeadingTokenException(this, parent.Previous);

					ColonName = name;
					parent.PopPrevious();
					break;

				// Closing parentases should be caught by the opening ones, otherwise they're stray
				default:
					throw new ParseUnexpectedTokenException(this);
			}
		}

		public void ConvertIntoFunctionCall(IteratedList<Token> parent)
		{
			if (!IsOpenParentasesOfChar(this, '(')) return;

			Token prev = parent.Previous;

			// Merge with identifier, this is a function call
			if (OperatorToken.ExpressionHasValue(prev))
			{
				parent.PopPrevious(); // id
				parent.SwapCurrent(new FunctionCallToken(source, prev, this));
				parent.ParseTokenAt(parent.Index);
			}
			else if (Count == 0)
				throw new ParseTokenException("Unexpected empty parentases group <()>.", this);
		}

		private void CollectUntilClosingParentases(IteratedList<Token> parent)
		{
			var nestedParentases = 0;
			// Collect all until group found
			while (true)
			{
				Token next = parent.Next;
				switch (next)
				{
					case null:
						throw new ParseTokenException($"Unexpected EOF, expected <{GetMatchingParentases(Character)}>!", this);

					case PunctuatorToken open when IsOpenParentasesOfChar(open, Character):
						nestedParentases++;
						goto default;

					case PunctuatorToken close when close.PunctuatorType == Type.ClosingParentases
						&& close.Character == GetMatchingParentases(Character):
						if (nestedParentases == 0)
						{
							parent.PopNext();
							return; // stops
						}

						nestedParentases--;
						goto default;

					default:
						this.Add(parent.PopNext());
						break;
				}
			}
		}

		public static bool IsPunctuatorOfType(Token token, Type type)
		{
			return token is PunctuatorToken pun
			       && pun.PunctuatorType == type;
		}

		public static bool IsSeparatorOfChar(Token token, char sepChar)
		{
			return token is PunctuatorToken pun
			       && pun.PunctuatorType == Type.Separator
			       && pun.Character == sepChar;
		}

		public static bool IsOpenParentasesOfChar(Token token, char openChar)
		{
			return token is PunctuatorToken pun
				   && pun.PunctuatorType == Type.OpeningParentases
				   && pun.Character == openChar;
		}

		public static char GetMatchingParentases(char c)
		{
			return parentasesPairs[c];
		}

		public override string ToString()
		{
			switch (PunctuatorType)
			{
				case Type.Dot:
					return DotLHS + base.ToString() + DotRHS;

				case Type.Colon:
					return ColonName + base.ToString() + TrailingWhitespace;

				case Type.OpeningParentases:
					if (Count == 0)
						return Character.ToString() + GetMatchingParentases(Character);
					
					return Character + this.Aggregate(this[0].LeadingWhitespace, (s, t) => s + t + t.TrailingWhitespace) + GetMatchingParentases(Character);

				default:
					return base.ToString();
			}
		}

		public enum Type
		{
			OpeningParentases,
			ClosingParentases,
			Separator,
			Dot,
			Colon,
		}
	}
}