using System;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;

namespace RobotPlusPlus.Linguist.Exceptions
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