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
	/// <summary>Reserved words. Ex: if, while, try</summary>
	public class StatementToken : Token
	{
		public Token Condition
		{
			get => this[0];
			set => this[0] = value;
		}

		public Token CodeBlock
		{
			get => this[1];
			set => this[1] = value;
		}

		public Token ElseBlock
		{
			get => this[2];
			set => this[2] = value;
		}

		public Type StatementType { get; }

		public StatementToken(TokenSource source) : base(source)
		{
			if (Enum.TryParse(SourceCode, true, out Type statementType))
				StatementType = statementType;
			else
				throw new ParseUnexpectedTokenException(this);
		}

		public override void ParseToken(IteratedList<Token> parent)
		{
			if (StatementType == Type.Else)
				throw new ParseUnexpectedTokenException(this);

			Condition = ParseTokenCondition(parent);
			CodeBlock = ParseTokenCodeBlock(parent);
			ElseBlock = ParseTokenElseBlock(parent);
		}

		private Token ParseTokenCondition(IteratedList<Token> parent)
		{
			Token next = parent.Next;

			switch (StatementType)
			{
				case Type.While:
				case Type.If:
					if (OperatorToken.ExpressionHasValue(next))
						return parent.PopNext();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);

				default:
					throw new InvalidOperationException("Unknown statement type!");
			}
		}

		private Token ParseTokenCodeBlock(IteratedList<Token> parent)
		{
			Token next = parent.Next;

			switch (StatementType)
			{
				case Type.While:
				case Type.If:
					if (next is StatementToken st)
					{
						if (st.StatementType == Type.Else)
							throw new ParseUnexpectedTrailingTokenException(this, st);

						parent.ParseNextToken();
					}

					if (PunctuatorToken.IsOpenParentasesOfChar(next, '{')
					    || (next is OperatorToken op && op.OperatorType == OperatorToken.Type.Assignment)
					    || next is StatementToken)
						return parent.PopNext();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);

				default:
					throw new InvalidOperationException("Unknown statement type!");
			}
		}

		private Token ParseTokenElseBlock(IteratedList<Token> parent)
		{
			switch (StatementType)
			{
				case Type.If:
					if (!(parent.Next is StatementToken next)) return null;
					if (next.StatementType != Type.Else) return null;

					parent.PopNext();
					return ParseTokenCodeBlock(parent);

				default:
					return null;
			}
		}

		public enum Type
		{
			If,
			Else,
			While,
			Unknown,
		}
	}
}