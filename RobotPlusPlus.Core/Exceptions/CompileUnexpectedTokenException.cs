using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileUnexpectedTokenException : CompileException
	{
		public CompileUnexpectedTokenException(Token source) : this(source, null)
		{ }

		public CompileUnexpectedTokenException(Token source, Exception innerException)
			: base($"Unexpected {source.GetType().Name} token while compiling, value <{source.SourceCode}>.", source, innerException)
		{ }
	}
}