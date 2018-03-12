using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public abstract class Literal : Token
	{
		protected Literal(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{ }
	}
}