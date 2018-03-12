using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Operator : Token
	{
		public Type OperatorType { get; }

		public Operator(Token token) : this(token.SourceCode, token.SourceLine) { }

		public Operator(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			switch (sourceCode)
			{
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

				case "==":
				case ">=":
				case "<=":
				case ">":
				case "<":
				case "!=":
					OperatorType = Type.Comparator;
					break;

				case "+":
				case "++":
				case "-":
				case "--":
				case "*":
				case "/":
				case "%":
				case "&":
				case "|":
				case "^":
				case "<<":
				case ">>":
					OperatorType = Type.Math;
					break;

				case "!":
				case "~":
					OperatorType = Type.Unary;
					break;

				default:
					throw new ParseException($"Unregistered operator type <{sourceCode}>", sourceLine);

			}
		}

		public enum Type
		{
			Assignment,
			Comparator,
			Unary,
			Math,
		}

		public override void AssertToken(Asserter asserter)
		{
			throw new System.NotImplementedException();
		}
	}
}