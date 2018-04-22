using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeReadOnlyException : CompileException
	{
		public CompileTypeReadOnlyException(IdentifierToken variable) : this(variable, null)
		{ }

		public CompileTypeReadOnlyException(IdentifierToken variable, Exception innerException)
			: base($"Cannot assign value to readonly type <{variable.Identifier}>.", variable, innerException)
		{ }
	}
}