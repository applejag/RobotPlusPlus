using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Punctuator : Token
	{
		public Punctuator(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
		}

		public override void AssertToken(Asserter asserter)
		{}
	}
}