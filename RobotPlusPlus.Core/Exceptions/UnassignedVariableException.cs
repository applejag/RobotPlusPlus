using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
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