using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Ignored code. Ex: //line, /*block*/</summary>
	public class Comment : Token
	{
		public bool IsBlockComment { get; }
		
		public Comment(string sourceCode, int sourceLine, bool isBlock) : base(sourceCode, sourceLine)
		{
			IsBlockComment = isBlock;
		}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return string.Empty;
		}
	}
}