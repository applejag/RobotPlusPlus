using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
		[CanBeNull]
		public WhitespaceToken LeadingWhitespaceToken { get; set; }
		[CanBeNull]
		public WhitespaceToken TrailingWhitespaceToken { get; set; }
		[NotNull]
		public string LeadingWhitespace => LeadingWhitespaceToken?.ToString() ?? "";
		[NotNull]
		public string TrailingWhitespace => TrailingWhitespaceToken?.ToString() ?? "";
		public bool IsParsed { get; internal set; }

		protected Token(TokenSource source)
		{
			this.source = source;
		}

		public abstract void ParseToken(IteratedList<Token> parent);

		public override string ToString()
		{
			return SourceCode;
		}

	}
}