using System;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;

namespace RobotPlusPlus.Linguist.Exceptions
{
	public class ParseTokenException : ParseException
	{
		public Token Token { get; }

		public ParseTokenException(string message, Token source)
			: this(message, source, null)
		{ }

		public ParseTokenException(string message, Token source, Exception innerException)
			: base(message, source.SourceLine, innerException)
		{
			Token = source;
		}
	}
}