using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public abstract class Token
	{
		public string SourceCode { get; }
		public int SourceLine { get; }
		public int NewLines { get; }

		public List<Token> Tokens = new List<Token>();

		protected Token(string sourceCode, int sourceLine)
		{
			SourceCode = sourceCode;
			SourceLine = sourceLine;

			var newLines = 0;
			foreach (char c in sourceCode)
			{
				if (c == '\n')
					newLines++;
			}

			NewLines = newLines;
		}

		public abstract void ParseToken(Parser parser);

	}
}