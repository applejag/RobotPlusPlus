using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Identifier : Token
	{
		public Identifier(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
		}

		public override void ParseToken(Parser parser)
		{ }
	}
}