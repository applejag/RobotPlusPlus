using System;
using System.Linq;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Tokenizing
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
		
	}
}