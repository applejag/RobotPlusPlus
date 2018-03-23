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

		public override void ParseToken(IList<Token> parent, int myIndex)
		{
			int prevIndex = myIndex - 1;
			int nextIndex = myIndex + 1;
			Token prev = parent.TryGet(prevIndex);
			Token next = parent.TryGet(nextIndex);

			void TakePrevForLHS()
			{
				LHS = parent.Pop(prevIndex);
				nextIndex--;
				myIndex--;
			}

			void TakeNextForRHS()
			{
				RHS = parent.Pop(nextIndex);
			}

			switch (OperatorType)
			{
				// Expression
				case Type.Expression when prev is IdentifierToken:
					TakePrevForLHS();
					break;
				case Type.Expression:
					throw new NotImplementedException("Expressions are not yet implemented! (ex: ++x, --x)");

				// Unary
				case Type.Unary:
					TakeNextForRHS();
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
						TakePrevForLHS();
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					if (ExpressionHasValue(next))
						TakeNextForRHS();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;

				case Type.Assignment:
					if (prev is IdentifierToken)
						TakePrevForLHS();
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					// ex: <<=, +=, %=
					// Add identifier & operator to pool
					if (SourceCode != "=")
					{
						// Duplicate identifier & create operand from my source
						var id = new IdentifierToken(LHS.source);
						var op = new OperatorToken(new TokenSource(source.code.Substring(0, SourceCode.Length - 1), source.file, source.line,
							source.column));

						// Add to parent
						parent.Insert(nextIndex + 0, id);
						parent.Insert(nextIndex + 1, op);

						// Parse
						parent.ParseTokenAt(nextIndex + 1);
					}

					if (ExpressionHasValue(next))
						TakeNextForRHS();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;

				default:
					throw new ParseUnexpectedTokenException(this);
			}
		}

		public override string CompileToken(Compiler compiler)
		{
			switch (OperatorType)
			{
				case Type.Assignment:
					if (this.AnyRecursive(t => t is LiteralNumberToken)
					    && this.AnyRecursive(t => t is LiteralStringToken))
						compiler.assignmentNeedsCSSnipper = true;

					string c_rhs = RHS.CompileToken(compiler);
					compiler.RegisterVariable(LHS as IdentifierToken ??
					                          throw new CompileException("Missing identifier for assignment.", this));
					string c_lhs = LHS.CompileToken(compiler);

					string formatString = compiler.assignmentNeedsCSSnipper
						? "{0}=⊂{1}⊃"
						: "{0}={1}";

					return string.Format(formatString, c_lhs, c_rhs);

				default:
					return string.Format("{0}{1}{2}", LHS?.CompileToken(compiler), SourceCode, RHS?.CompileToken(compiler));
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