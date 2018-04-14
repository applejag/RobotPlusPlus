using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileVariableTypeConflictException : CompileException
	{
		public Type ExpectedType { get; }
		public Type ActualType { get; }

		public CompileVariableTypeConflictException(IdentifierToken variable, Type expected, Type actual) : this(variable, expected, actual, null)
		{ }

		public CompileVariableTypeConflictException(IdentifierToken variable, Type expected, Type actual,
			Exception innerException)
			: base(
				$"Cannot implicitly convert <{actual.Name}> to <{expected.Name}> when assigning value to <{variable.Identifier}>.",
				variable, innerException)
		{
			ExpectedType = expected;
			ActualType = actual;
		}
	}
}