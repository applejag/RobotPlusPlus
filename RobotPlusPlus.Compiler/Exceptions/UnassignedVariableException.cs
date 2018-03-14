using System;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Exceptions
{
	public class UnassignedVariableException : CompileException
	{
		public UnassignedVariableException(Token variable) : this(variable, null)
		{ }

		public UnassignedVariableException(Token variable, Exception innerException)
			: base($"Use of unassigned variable <{variable.SourceCode}>.", variable, innerException)
		{ }
	}
}