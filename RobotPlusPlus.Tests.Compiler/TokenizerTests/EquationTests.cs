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
				typeof(Identifier),   // y
				typeof(Operator),     // =
				typeof(Identifier),   // x
				typeof(Operator),     // *
				typeof(Identifier),   // x
				typeof(Operator),     // +
				typeof(Punctuator),   // (
				typeof(Literal),      // 50
				typeof(Operator),     // -
				typeof(Identifier),   // z
				typeof(Punctuator),   // )
				typeof(Operator),     // *
				typeof(Literal)       // .5
				);
		}
	}
}