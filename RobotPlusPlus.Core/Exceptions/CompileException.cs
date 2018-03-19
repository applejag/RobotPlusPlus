using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
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