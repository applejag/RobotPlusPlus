using System;

namespace RobotPlusPlus.Core.Exceptions
{
	public class ParseException : ApplicationException
	{
		public int Line { get; }
		public int Column { get; }

		public ParseException(string msg, int line, int column, Exception innerException)
			: base(msg, innerException)
		{
			Line = line;
			Column = column;
		}

		public ParseException(string msg, int line, int column)
			: this(msg, line, column, null)
		{ }
	}
}