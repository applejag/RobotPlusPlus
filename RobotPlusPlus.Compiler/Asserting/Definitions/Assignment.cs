using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Asserting.Definitions
{
	public class Assignment : CodeUnit
	{
		public Token TargetIdentifier { get; }
		public OperatorToken Operator { get; }
		public Expression Expression { get; }

		public Assignment(Asserter asserter)
		{
			TargetIdentifier = asserter.NextToken();
			Operator = (OperatorToken)asserter.NextToken();
			Expression = new Expression().ReadUntilSemicolon(asserter);
		}
	}
}