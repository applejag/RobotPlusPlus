using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Token
	{
		public TokenType Type { get; }
		public string SourceCode { get; }
		public int SourceLine { get; }

		public Token(TokenType type, string sourceCode, int sourceLine)
		{
			Type = type;
			SourceCode = sourceCode;
			SourceLine = sourceLine;
		}

		public virtual void AssertToken(Asserter asserter)
		{

		}

	}
}