using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Statement : Token
	{
		public Statement(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
		}

		public override void ParseToken(Parser parser)
		{ }
	}
}