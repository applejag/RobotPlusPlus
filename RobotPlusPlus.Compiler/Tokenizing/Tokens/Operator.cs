using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Assignment and comparisson. Ex: =, >, +</summary>
	public class Operator : Token
	{
		public Type OperatorType { get; }
		public Token LHS
		{
			get => Tokens[0];
			set => Tokens[0] = value;
		}
		public Token RHS
		{
			get => Tokens[1];
			set => Tokens[1] = value;
		}

		public Operator(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			Tokens = new List<Token> { null, null };

			switch (sourceCode)
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
					throw new ParseException($"Unregistered operator type <{sourceCode}>", this);

			}
		}

		public override void ParseToken(Parser parser)
		{
			Token prev = parser.PrevToken;
			Token next = parser.NextToken;

			switch (OperatorType)
			{
				// Expression
				case Type.Expression when prev is Identifier:
					LHS = parser.TakePrevToken();
					break;
				case Type.Expression:
					throw new ParseUnexpectedLeadingTokenException(this, prev);

				// Unary
				case Type.Unary:
					RHS = parser.TakeNextToken();
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
					if (prev is Identifier
						|| prev is Literal
						|| (prev is Punctuator lp && lp.PunctuatorType == Punctuator.Type.OpeningParentases))
						LHS = parser.TakePrevToken();
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					if (next is Identifier
						|| next is Literal
						|| (next is Punctuator tp && tp.PunctuatorType == Punctuator.Type.OpeningParentases))
						RHS = parser.TakeNextToken();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;

				case Type.Assignment:
					if (prev is Identifier)
						LHS = parser.TakePrevToken();
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					if (next is Identifier
						|| next is Literal
						|| (next is Punctuator tp2 && tp2.PunctuatorType == Punctuator.Type.OpeningParentases))
						RHS = parser.TakeNextToken();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;
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