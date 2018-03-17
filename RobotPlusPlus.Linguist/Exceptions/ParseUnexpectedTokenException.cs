using System;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;

namespace RobotPlusPlus.Linguist.Exceptions
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