using System;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus
{
	public class ParseUnexpectedLeadingTokenException : ParseException
	{
		public ParseUnexpectedLeadingTokenException(Token source, Token leading)
			: this(source, leading, null)
		{ }

		public ParseUnexpectedLeadingTokenException(Token source, Token leading, Exception innerException)
			: base($"Unexpected leading token <{leading.GetType().Name}, {leading.SourceCode}> before <{source.SourceCode}>.", leading, innerException)
		{ }
	}
}