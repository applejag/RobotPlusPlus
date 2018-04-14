using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling
{
	public static class OperatorTypeCheckingExtensions
	{
		public static Type EvaluateType(this OperatorToken token, Type RHS)
		{
			switch (token.SourceCode)
			{
				case "+": return TypeChecking.CanUnaryAdditive(RHS);
				case "-": return TypeChecking.CanUnarySubtractive(RHS);
				case "~": return TypeChecking.CanUnaryBitwiseNegate(RHS);
				case "!": return TypeChecking.CanNegate(RHS);

				default:
					throw new NotImplementedException();
			}
		}

		public static Type EvaluateType(this OperatorToken token, Type LHS, Type RHS)
		{
			switch (token.SourceCode)
			{
				case "+": return TypeChecking.CanAdd(LHS, RHS);
				case "-": return TypeChecking.CanSubtract(LHS, RHS);

				case ">": return TypeChecking.CanGreaterThan(LHS, RHS);
				case ">=": return TypeChecking.CanGreaterThanOrEqual(LHS, RHS);
				case "<": return TypeChecking.CanLessThan(LHS, RHS);
				case "<=": return TypeChecking.CanLessThanOrEqual(LHS, RHS);

				case "*": return TypeChecking.CanMultiplicate(LHS, RHS);
				case "/": return TypeChecking.CanDivide(LHS, RHS);
				case "%": return TypeChecking.CanModulus(LHS, RHS);

				case "==": return TypeChecking.CanEqual(LHS, RHS);
				case "!=": return TypeChecking.CanNotEqual(LHS, RHS);

				default:
					throw new NotImplementedException();
			}
		}
	}
}