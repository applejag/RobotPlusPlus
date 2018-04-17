using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeInvalidOperationException : CompileException
	{
		public Type LHS { get; }
		public Type RHS { get; }
		public Type Unary { get; }

		public CompileTypeInvalidOperationException(OperatorToken token, Type unary)
			: this(token, unary, innerException: null)
		{ }

		public CompileTypeInvalidOperationException(OperatorToken token, Type unary, Exception innerException)
			: base($"Operator {token.OperatorType} <{token.SourceCode}> cannot be applied to <{unary}>.", token, innerException)
		{
			Unary = unary;
		}

		public CompileTypeInvalidOperationException(OperatorToken token, Type LHS, Type RHS)
			: this(token, LHS, RHS, null)
		{ }

		public CompileTypeInvalidOperationException(OperatorToken token, Type LHS, Type RHS,
			Exception innerException)
			: base(
				$"Operator {token.OperatorType} <{token.SourceCode}> cannot be applied to <{RHS.Name}> and <{LHS.Name}>.",
				token, innerException)
		{
			this.LHS = LHS;
			this.RHS = RHS;
		}
	}
}