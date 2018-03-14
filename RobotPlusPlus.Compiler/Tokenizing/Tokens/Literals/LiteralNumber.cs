using System.Globalization;
using RobotPlusPlus.Parsing;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Tokenizing.Tokens.Literals
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
			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (sourceCode.IndexOf('.') == -1)
				Value = int.Parse(sourceCode, NumberStyles.None, CultureInfo.InvariantCulture);
			else
				Value = double.Parse(sourceCode, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
		}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken()
		{
			return IsReal
				? RealValue.ToString("0.0#############################", CultureInfo.InvariantCulture)
				: IntegerValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}