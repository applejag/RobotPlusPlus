using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Variables. Ex: x, myValue, go_johnny_go</summary>
	public class Identifier : Token
	{
		public Identifier(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
		}

		public override void ParseToken(Parser parser)
		{ }
	}
}