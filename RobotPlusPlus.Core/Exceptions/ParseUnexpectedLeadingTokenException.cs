using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseUnexpectedLeadingTokenException : ParseTokenOtherException
	{
		public ParseUnexpectedLeadingTokenException(Token source, Token leading)
			: this(source, leading, null)
		{ }

		public ParseUnexpectedLeadingTokenException(Token source, Token leading, Exception innerException)
			: base(
				$"Unexpected leading token <{(leading == null ? "null" : leading.GetType().Name + ", " + leading.SourceCode.EscapeString())}> before <{source.SourceCode.EscapeString()}>.",
				source, leading, innerException)
		{ }
	}
}