using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Assignment and comparisson. Ex: =, >, +</summary>
	public class Operator : Token
	{
		public Type OperatorType { get; }
		public Token LHS => Tokens[_LHS];
		public Token RHS => Tokens[_RHS];
		public const int _LHS = 0;
		public const int _RHS = 1;

		public bool ContainsValue => Tokens.Any(t =>
			t is Literal
			|| t is Identifier
			|| (t is Punctuator p && p.PunctuatorType == Punctuator.Type.OpeningParentases &&
			    p.Tokens.Count > 0)
			|| (t is Operator o && o.ContainsValue));

		public Operator(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
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
					parser.TakePrevToken(_LHS); // LHS
					break;
				case Type.Expression:
					throw new ParseUnexpectedLeadingTokenException(this, prev);

				// Unary
				case Type.Unary:
					parser.TakeNextToken(_RHS);
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
						|| (prev is Punctuator lp && lp.PunctuatorType == Punctuator.Type.OpeningParentases)
					    || (prev is Operator op1 && op1.ContainsValue))
						parser.TakePrevToken(_LHS);
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					if (next is Identifier
						|| next is Literal
						|| (next is Punctuator tp && tp.PunctuatorType == Punctuator.Type.OpeningParentases)
					    || (next is Operator op2 && op2.ContainsValue))
						parser.TakeNextToken(_RHS);
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;

				case Type.Assignment:
					if (prev is Identifier)
						parser.TakePrevToken(_LHS);
					else
						throw new ParseUnexpectedLeadingTokenException(this, prev);

					if (next is Identifier
						|| next is Literal
						|| (next is Punctuator tp2 && tp2.PunctuatorType == Punctuator.Type.OpeningParentases)
						|| (next is Operator op3 && op3.ContainsValue))
						parser.TakeNextToken(_RHS);
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