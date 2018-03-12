using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Punctuator : Token
	{
		public Punctuator(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
		}

		public override void ParseToken(Parser parser)
		{}
	}
}