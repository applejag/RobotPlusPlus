using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseUnexpectedTrailingTokenException : ParseTokenException
	{
		public ParseUnexpectedTrailingTokenException(Token source, Token trailing)
			: this(source, trailing, null)
		{ }

		public ParseUnexpectedTrailingTokenException(Token source, Token trailing, Exception innerException)
			: base($"Unexpected trailing token <{(trailing == null ? "null" : trailing.GetType().Name + ", " + trailing.SourceCode)}> after <{source.SourceCode}>.",
				trailing ?? source, innerException)
		{ }
	}
}