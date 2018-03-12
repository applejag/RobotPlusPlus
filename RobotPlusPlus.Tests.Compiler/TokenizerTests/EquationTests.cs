using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Tests.TokenizerTests
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
			Utility.AssertTokenTypes(result,
				TokenType.Identifier,	// y
				TokenType.Operator,     // =
				TokenType.Identifier,   // x
				TokenType.Operator,     // *
				TokenType.Identifier,   // x
				TokenType.Operator,     // +
				TokenType.Punctuator,  // (
				TokenType.Literal,      // 50
				TokenType.Operator,     // -
				TokenType.Identifier,   // z
				TokenType.Punctuator,  // )
				TokenType.Operator,     // *
				TokenType.Literal       // .5
				);
		}
	}
}