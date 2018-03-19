﻿using System.Globalization;
using RobotPlusPlus.Linguist.Compiling;
using RobotPlusPlus.Linguist.Parsing;
using RobotPlusPlus.Linguist.Utility;

namespace RobotPlusPlus.Linguist.Tokenizing.Tokens.Literals
{
	public class LiteralNumber : Literal
	{
		/// <summary><see cref="Value"/> is <seealso cref="double"/></summary>
		public bool IsReal => Value is double;

		public double RealValue => (double) Value;

		/// <summary><see cref="Value"/> is <seealso cref="int"/></summary>
		public bool IsInteger => Value is int;

		public int IntegerValue => (int) Value;

		public object Value { get; }

		public LiteralNumber(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			if (sourceCode.EndsWith('f', ignoreCase: true))
				Value = double.Parse(sourceCode.Substring(0, sourceCode.Length - 1), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
			else if (sourceCode.IndexOf('.') == -1)
				Value = int.Parse(sourceCode, NumberStyles.None, CultureInfo.InvariantCulture);
			else
				Value = double.Parse(sourceCode, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
		}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return IsReal
				? RealValue.ToString("0.0#############################", CultureInfo.InvariantCulture)
				: IntegerValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}