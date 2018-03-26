using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseUnexpectedTrailingTokenException : ParseTokenOtherException
	{
		public ParseUnexpectedTrailingTokenException(Token source, Token trailing)
			: this(source, trailing, null)
		{ }

		public ParseUnexpectedTrailingTokenException(Token source, Token trailing, Exception innerException)
			: base($"Unexpected trailing token <{(trailing == null ? "null" : trailing.GetType().Name + ", " + trailing.SourceCode.EscapeString())}> after <{source.SourceCode.EscapeString()}>.",
				source, trailing, innerException)
		{ }
	}
}