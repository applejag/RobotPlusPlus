using System.Globalization;
using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens.Literals
{
	public class LiteralNumber : Literal
	{
		public bool IsReal => Value is double;
		public bool IsInteger => Value is int;
		public object Value { get; }

		public LiteralNumber(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			Value = sourceCode.IndexOf('.') == -1
				? int.Parse(sourceCode, NumberStyles.None, CultureInfo.InvariantCulture)
				: double.Parse(sourceCode, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
		}

		public override void AssertToken(Asserter asserter)
		{}
	}
}