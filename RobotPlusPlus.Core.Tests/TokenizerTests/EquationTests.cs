using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.TokenizerTests
{
	[TestClass]
	public class EquationTests
	{
		[TestMethod]
		public void Tokenize_SimpleEquation()
		{
			// Arrange
			const string input = "y=x*x+(50-z)*.5";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(IdentifierToken),   // y
				typeof(OperatorToken),     // =
				typeof(IdentifierToken),   // x
				typeof(OperatorToken),     // *
				typeof(IdentifierToken),   // x
				typeof(OperatorToken),     // +
				typeof(PunctuatorToken),   // (
				typeof(LiteralToken),      // 50
				typeof(OperatorToken),     // -
				typeof(IdentifierToken),   // z
				typeof(PunctuatorToken),   // )
				typeof(OperatorToken),     // *
				typeof(LiteralToken)       // .5
				);
		}
	}
}