using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	public abstract class Token : FlexibleList<Token>
	{
		protected internal readonly TokenSource source;
		public string SourceCode => source.code;
		public int SourceLine => source.line;
		public int SourceColumn => source.column;
		public int NewLines => source.NewLines;
		public WhitespaceToken LeadingWhitespaceToken { get; set; }
		public WhitespaceToken TrailingWhitespaceToken { get; set; }
		public bool IsParsed { get; internal set; }

		protected Token(TokenSource source)
		{
			this.source = source;
		}

		public abstract void ParseToken(IList<Token> parent, int myIndex);
		public abstract string CompileToken(Compiler compiler);

		public override string ToString()
		{
			return Count == 0 
				? SourceCode
				: $"{SourceCode}[{string.Join(", ", this.Select(t => t?.ToString() ?? "null"))}]";
		}

	}
}