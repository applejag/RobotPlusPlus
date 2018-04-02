using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileFunctionException : CompileException
	{
		public CompileFunctionException(string msg, Token source)
			: this(msg, source, null)
		{ }

		public CompileFunctionException(string msg, Token source, Exception innerException)
			: base(msg, source, innerException)
		{ }
	}
}