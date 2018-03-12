using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Whitespace : Token
	{
		public Whitespace(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{}

		public override void ParseToken(Parser parser)
		{ }
	}
}