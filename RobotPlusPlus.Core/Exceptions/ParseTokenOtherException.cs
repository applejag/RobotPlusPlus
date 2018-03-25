using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseTokenOtherException : ParseTokenException
	{
		public Token Other { get; }

		public ParseTokenOtherException(string message, Token source, Token other)
			: this(message, source, other, null)
		{ }

		public ParseTokenOtherException(string message, Token source, Token other, Exception innerException)
			: base(message, source, innerException)
		{
			Other = other;
		}
	}
}