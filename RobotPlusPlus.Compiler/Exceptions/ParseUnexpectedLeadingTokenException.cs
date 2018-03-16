using System;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Exceptions
{
	public class ParseUnexpectedLeadingTokenException : ParseTokenException
	{
		public ParseUnexpectedLeadingTokenException(Token source, Token leading)
			: this(source, leading, null)
		{ }

		public ParseUnexpectedLeadingTokenException(Token source, Token leading, Exception innerException)
			: base(
				$"Unexpected leading token <{(leading == null ? "null" : leading.GetType().Name + ", " + leading.SourceCode)}> before <{source.SourceCode}>.",
				leading ?? source, innerException)
		{ }
	}
}