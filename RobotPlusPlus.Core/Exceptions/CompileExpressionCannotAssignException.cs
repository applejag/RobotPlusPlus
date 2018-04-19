using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileExpressionCannotAssignException : CompileException
	{
		public CompileExpressionCannotAssignException(Token token)
			: this(token, null)
		{ }

		public CompileExpressionCannotAssignException(Token token,
			Exception innerException)
			: base(
				$"You cannot assign a value to <{token?.GetType().Name ?? "null"}>, <{token?.SourceCode ?? "null"}>.",
				token, innerException)
		{ }
	}
}