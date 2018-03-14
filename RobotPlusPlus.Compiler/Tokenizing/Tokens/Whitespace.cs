using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Spaces and newlines. Ex: \n, \t, \r</summary>
	public class Whitespace : Token
	{
		public Whitespace(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken()
		{
			return string.Empty;
		}
	}
}