using System;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;

namespace RobotPlusPlus.Linguist.Exceptions
{
	public class CompileException : ParseTokenException
	{
		public CompileException(string msg, Token source)
			: base(msg, source)
		{ }

		public CompileException(string msg, Token source, Exception innerException)
			: base(msg, source, innerException)
		{ }
	}
}