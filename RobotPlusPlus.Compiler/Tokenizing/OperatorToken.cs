﻿namespace RobotPlusPlus.Tokenizing
{
	public class OperatorToken : Token
	{
		
		public Type OperatorType { get; }

		public OperatorToken(string sourceCode, int sourceLine) : base(TokenType.Operator, sourceCode, sourceLine)
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

		public new enum Type
		{
			Assignment,
			Comparator,
			Unary,
			Math,
		}
	}
}