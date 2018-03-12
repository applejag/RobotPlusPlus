﻿using System.Globalization;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens.Literals
{
	public class LiteralNumber : Literal
	{
		public bool IsReal => Value is double;
		public bool IsInteger => Value is int;
		public object Value { get; }

		public LiteralNumber(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (sourceCode.IndexOf('.') == -1)
				Value = int.Parse(sourceCode, NumberStyles.None, CultureInfo.InvariantCulture);
			else
				Value = double.Parse(sourceCode, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
		}

		public override void ParseToken(Parser parser)
		{}
	}
}