using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeConvertImplicitAssignmentException : CompileTypeConvertImplicitException
	{
		public CompileTypeConvertImplicitAssignmentException(IdentifierToken variable, Type from, Type to) : this(variable, to, @from, null)
		{ }

		public CompileTypeConvertImplicitAssignmentException(IdentifierToken variable, Type from, Type to,
			Exception innerException)
			: base(
				$"Cannot implicitly convert <{from.Name}> to <{to.Name}> when assigning value to <{variable.Identifier}>.",
				variable, to, from, innerException)
		{
		}
	}
}