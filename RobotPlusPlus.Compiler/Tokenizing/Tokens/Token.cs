using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public abstract class Token
	{
		public string SourceCode { get; }
		public int SourceLine { get; }
		public int NewLines { get; }
		public Whitespace LeadingWhitespace { get; set; }
		public Whitespace TrailingWhitespace { get; set; }

		public List<Token> Tokens = new List<Token>();
		public bool IsParsed { get; internal set; }

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

		public Token this[int index]
		{
			get => Tokens[index];
			set => Tokens[index] = value;
		}

		public abstract void ParseToken(Parser parser);

		public override string ToString()
		{
			return SourceCode == null
				? $"{{{GetType().Name}, source=null}}"
				: $"{{{GetType().Name}, source=\"{SourceCode}\"}}";
		}
	}
}