using System.Globalization;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens.Literals
{
	public class LiteralNumber : Literal
	{
		/// <summary><see cref="Value"/> is <seealso cref="double"/></summary>
		public bool IsReal => Value is double;
		/// <summary><see cref="Value"/> is <seealso cref="int"/></summary>
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