using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileIncorrectTokenCountException : CompileException
	{
		public int ExpectedCount { get; }
		public int ActualCount { get; }

		public CompileIncorrectTokenCountException(int expected, Token source)
			: this(expected, source, null)
		{ }

		public CompileIncorrectTokenCountException(int expected, Token source, Exception innerException)
			: base($"Incorrect token count contained in <{source.SourceCode}>. Expected {expected}, got {source.Count}.", source, innerException)
		{
			ExpectedCount = expected;
			ActualCount = Token.Count;
		}
	}
}