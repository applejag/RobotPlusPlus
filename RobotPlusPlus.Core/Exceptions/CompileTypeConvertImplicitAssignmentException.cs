using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeConvertImplicitAssignmentException : CompileTypeConvertImplicitException
	{
		public CompileTypeConvertImplicitAssignmentException(IdentifierToken variable, Type expected, Type actual) : this(variable, expected, actual, null)
		{ }

		public CompileTypeConvertImplicitAssignmentException(IdentifierToken variable, Type expected, Type actual,
			Exception innerException)
			: base(
				$"Cannot implicitly convert <{actual.Name}> to <{expected.Name}> when assigning value to <{variable.Identifier}>.",
				variable, expected, actual, innerException)
		{
		}
	}
}