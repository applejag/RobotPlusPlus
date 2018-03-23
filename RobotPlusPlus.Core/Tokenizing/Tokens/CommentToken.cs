using System.Collections.Generic;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Ignored code. Ex: //line, /*block*/</summary>
	public class CommentToken : Token
	{
		public bool IsBlockComment { get; }
		
		public CommentToken(TokenSource source, bool isBlock) : base(source)
		{
			IsBlockComment = isBlock;
		}

		public override void ParseToken(IteratedList<Token> parent)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return string.Empty;
		}
	}
}