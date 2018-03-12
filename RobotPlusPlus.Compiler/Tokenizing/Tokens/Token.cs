using RobotPlusPlus.Asserting;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public abstract class Token
	{
		public string SourceCode { get; }
		public int SourceLine { get; }
		public int NewLines { get; }

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

		public abstract void AssertToken(Asserter asserter);

	}
}