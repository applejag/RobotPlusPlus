using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Operator : Token
	{
		public Type OperatorType { get; }
		public Token LHS { get; private set; }
		public Token RHS { get; private set; }

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
					throw new ParseException($"Unregistered operator type <{sourceCode}>", sourceLine);

			}
		}

		public override void ParseToken(Parser parser)
		{
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