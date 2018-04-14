using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileExpressionTypeConflictException : CompileException
	{
		public Type ExpectedType { get; }
		public Type ActualType { get; }

		public CompileExpressionTypeConflictException(Token expression, Type expected, Type actual) : this(expression, expected, actual, null)
		{ }

		public CompileExpressionTypeConflictException(Token expression, Type expected, Type actual,
			Exception innerException)
			: base(
				$"Cannot implicitly convert <{actual.Name}> to <{expected.Name}> in expression.",
				expression, innerException)
		{
			ExpectedType = expected;
			ActualType = actual;
		}
	}
}