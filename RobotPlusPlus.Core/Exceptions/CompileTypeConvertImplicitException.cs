using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeConvertImplicitException : CompileException
	{
		public Type ExpectedType { get; }
		public Type ActualType { get; }

		public CompileTypeConvertImplicitException(Token expression, Type expected, Type actual) : this(expression, expected, actual, null)
		{ }

		public CompileTypeConvertImplicitException(Token expression, Type expected, Type actual,
			Exception innerException)
			: this(
				$"Cannot implicitly convert <{actual.Name}> to <{expected.Name}> in expression.",
				expression, expected, actual, innerException)
		{ }

		protected CompileTypeConvertImplicitException(string message, Token expression, Type expected, Type actual,
			Exception innerException)
			: base(message, expression, innerException)
		{
			ExpectedType = expected;
			ActualType = actual;
		}
	}
}