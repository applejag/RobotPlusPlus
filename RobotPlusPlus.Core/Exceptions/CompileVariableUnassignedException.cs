using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileVariableUnassignedException : CompileException
	{
		public CompileVariableUnassignedException(IdentifierToken variable) : this(variable, null)
		{ }

		public CompileVariableUnassignedException(IdentifierToken variable, Exception innerException)
			: base($"Use of unassigned variable <{variable.Identifier}>.", variable, innerException)
		{ }
	}
}