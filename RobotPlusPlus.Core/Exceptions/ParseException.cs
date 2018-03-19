using System;

namespace RobotPlusPlus.Core.Exceptions
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
	}
}