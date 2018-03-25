using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseUnexpectedTokenException : ParseTokenException
	{
		public ParseUnexpectedTokenException(Token source) : this(source, null)
		{ }

		public ParseUnexpectedTokenException(Token source, Exception innerException)
			: base($"Unexpected {source.GetType().Name} token while parsing, value <{source.SourceCode}>.", source, innerException)
		{ }
	}
}