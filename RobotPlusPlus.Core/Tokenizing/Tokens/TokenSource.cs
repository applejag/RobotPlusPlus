namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	public struct TokenSource
	{
		public string code;
		public string file;
		public int line;
		public int column;

		public int newLines;

		public TokenSource(string code, string file, int line, int column)
		{
			this.code = code;
			this.file = file;
			this.line = line;
			this.column = column;
			
			newLines = 0;

			foreach (char c in code)
			{
				if (c == '\n')
					newLines++;
			}
		}
	}
}