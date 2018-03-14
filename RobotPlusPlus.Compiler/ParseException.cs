using System;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus
{
	public class ParseException : ApplicationException
	{
		public int Line { get; }

		public ParseException(string msg, int line, Exception innerException)
			: base(msg, innerException)
		{
			Line = line;
		}

		public ParseException(string msg, int line)
			: this(msg, line, null)
		{ }

		public ParseException(string msg, Token variable)
			: this(msg, variable.SourceLine, null)
		{ }

		public ParseException(string msg, Token variable, Exception innerException)
			: this(msg, variable.SourceLine, innerException)
		{ }
	}
}