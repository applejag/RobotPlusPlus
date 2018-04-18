using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeConvertImplicitAssignmentException : CompileTypeConvertImplicitException
	{
		public CompileTypeConvertImplicitAssignmentException(IdentifierToken variable, Type from, Type to) : this(variable, from, to, null)
		{ }

		public CompileTypeConvertImplicitAssignmentException(IdentifierToken variable, Type from, Type to,
			Exception innerException)
			: base(
				$"Cannot implicitly convert <{from?.Name ?? "null"}> to <{to?.Name ?? "null"}> when assigning value to <{variable.Identifier}>.",
				variable, from, to, innerException)
		{
		}
	}
}