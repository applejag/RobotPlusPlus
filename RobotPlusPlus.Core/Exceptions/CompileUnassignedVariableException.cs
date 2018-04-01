using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileUnassignedVariableException : CompileException
	{
		public CompileUnassignedVariableException(Token variable) : this(variable, null)
		{ }

		public CompileUnassignedVariableException(Token variable, Exception innerException)
			: base($"Use of unassigned variable <{variable.SourceCode}>.", variable, innerException)
		{ }
	}
}