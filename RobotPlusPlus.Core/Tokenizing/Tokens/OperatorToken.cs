using System;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Assignment and comparisson. Ex: =, >, +</summary>
	public class OperatorToken : Token
	{
		public Type OperatorType { get; }

		public Token LHS
		{
			get => this[0];
			set => this[0] = value;
		}

		public Token RHS
		{
			get => this[1];
			set => this[1] = value;
		}

		public OperatorToken(TokenSource source) : base(source)
		{
			switch (SourceCode)
			{
				case "++":
				case "--":
					OperatorType = Type.Expression;
					break;

				case "!":
				case "~":
					OperatorType = Type.Unary;
					break;

				case "*":
				case "/":
				case "%":
					OperatorType = Type.Multiplicative;
					break;

				case "+":
				case "-":
					OperatorType = Type.Additive;
					break;

				case "<<":
				case ">>":
					OperatorType = Type.BitwiseShift;
					break;

				case ">=":
				case "<=":
				case ">":
				case "<":
					OperatorType = Type.Relational;
					break;

				case "==":
				case "!=":
					OperatorType = Type.Equality;
					break;

				case "&":
					OperatorType = Type.BitwiseAND;
					break;
				case "^":
					OperatorType = Type.BitwiseXOR;
					break;
				case "|":
					OperatorType = Type.BitwiseOR;
					break;

				case "&&":
					OperatorType = Type.BooleanAND;
					break;
				case "||":
					OperatorType = Type.BooleanAND;
					break;

				case "=":
				case "+=":
				case "-=":
				case "*=":
				case "/=":
				case "%=":
				case "^=":
				case "|=":
				case "&=":
				case "<<=":
				case ">>=":
					OperatorType = Type.Assignment;
					break;

				default:
					throw new ParseTokenException($"Unregistered operator type <{SourceCode}>", this);
			}
		}

		public static bool ExpressionHasValue(Token token)
		{
			switch (token)
			{
				case null:
					return false;

				case LiteralToken lit:
				case IdentifierToken id:
					return true;

				case OperatorToken op:
					if (op.OperatorType == Type.Unary)
						return ExpressionHasValue(op.LHS) && op.RHS == null;
					else
						return ExpressionHasValue(op.LHS) && ExpressionHasValue(op.RHS);

				case PunctuatorToken pun when pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases && pun.Character == '(':
					return pun.Any(ExpressionHasValue);

				default:
					return false;
			}
		}

		public override void ParseToken(IteratedList<Token> parent)
		{
			Token prev = parent.Previous;
			Token next = parent.Next;

			switch (OperatorType)
			{
				// Expression
				case Type.Expression when prev is IdentifierToken:
					LHS = parent.PopPrevious();
					break;
				case Type.Expression:
					throw new NotImplementedException("Expressions are not yet implemented! (ex: ++x, --x)");

				// Unary
				case Type.Unary:
					RHS = parent.PopNext();
					break;

				// Two sided expressions
				case Type.Multiplicative:
				case Type.Additive:
				case Type.BitwiseShift:
				case Type.Relational:
				case Type.Equality:
				case Type.BitwiseAND:
				case Type.BitwiseXOR:
				case Type.BitwiseOR:
				case Type.BooleanAND:
				case Type.BooleanOR:
					if (ExpressionHasValue(prev))
						LHS = parent.PopPrevious();
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					if (ExpressionHasValue(next))
						RHS = parent.PopNext();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;

				case Type.Assignment:
					if (prev is IdentifierToken)
						LHS = parent.PopPrevious();
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					// ex: <<=, +=, %=
					// Add identifier & operator to pool
					if (SourceCode != "=")
					{
						// Duplicate identifier & create operand from my source
						var id = new IdentifierToken(LHS.source);
						var op = new OperatorToken(new TokenSource(source.code.Substring(0, SourceCode.Length - 1), source.file,
							source.line,
							source.column));

						// Add to parent
						parent.PushRangeNext(id, op);

						// Parse operator
						parent.ParseTokenAt(parent.Index + 2);
					}

					if (ExpressionHasValue(next))
						RHS = parent.PopNext();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;

				default:
					throw new ParseUnexpectedTokenException(this);
			}
		}

		public enum Type
		{
			///<summary>++, --</summary>
			Expression,

			///<summary>-x, !x, ~x</summary>
			Unary,

			///<summary>x*y, x/y, x%y</summary>
			Multiplicative,

			///<summary>x+y, x-y</summary>
			Additive,

			///<summary>x&lt;&lt;y, x&gt;&gt;y</summary>
			BitwiseShift,

			///<summary>x&lt;y, x&gt;y, x&lt;=y, x&gt;=y</summary>
			Relational,

			///<summary>x==y, x!=y</summary>
			Equality,

			///<summary>x&amp;y</summary>
			BitwiseAND,

			///<summary>x^y</summary>
			BitwiseXOR,

			///<summary>x|y</summary>
			BitwiseOR,

			///<summary>x&amp;&amp;y</summary>
			BooleanAND,

			///<summary>x||y</summary>
			BooleanOR,

			/////<summary>x?true:false</summary>
			//Conditional,
			///<summary>x=y, x+=y, x-=y, x*=y, x/=y, etc.</summary>
			Assignment,
		}
	}
}