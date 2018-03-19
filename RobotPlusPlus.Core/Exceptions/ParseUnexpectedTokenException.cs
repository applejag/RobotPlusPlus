using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseUnexpectedTokenException : ParseTokenException
	{
		public ParseUnexpectedTokenException(Token source) : this(source, null)
		{ }

		public ParseUnexpectedTokenException(Token source, Exception innerException)
			: base($"Unexpected token value <{source.GetType().Name + ", " + source.SourceCode}>.", source, innerException)
		{ }
	}
}