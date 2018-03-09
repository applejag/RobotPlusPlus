using System;

namespace RobotPlusPlus
{
	public class ParseException : ApplicationException
	{
		public int Line { get; }

		public ParseException(string msg, int line)
			: base(msg)
		{
			Line = line;
		}

		public ParseException(string msg, int line, Exception innerException)
			: base(msg, innerException)
		{
			Line = line;
		}
	}
}