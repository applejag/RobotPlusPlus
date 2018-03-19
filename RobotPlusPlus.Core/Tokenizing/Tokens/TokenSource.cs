using System.Linq;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	public struct TokenSource
	{
		public string code;
		public string file;
		public int line;
		public int column;

		public int NewLines => code.Count(c => c == '\n');

		public TokenSource(string code, string file, int line, int column)
		{
			this.code = code;
			this.file = file;
			this.line = line;
			this.column = column;
		}
	}
}