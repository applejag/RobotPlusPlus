using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public class Comment : Token
	{
		public bool IsBlockComment { get; }
		
		public Comment(string sourceCode, int sourceLine, bool isBlock) : base(sourceCode, sourceLine)
		{
			IsBlockComment = isBlock;
		}

		public override void AssertToken(Asserter asserter)
		{

		}
	}
}