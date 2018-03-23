using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.TokenizerTests
{
	[TestClass]
	public class SimplePunctuatorTests
	{

		[TestMethod]
		public void Tokenize_ParenthesesSingle()
		{
			// Arrange
			const string input = "(";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(PunctuatorToken));
		}

		[TestMethod]
		public void Tokenize_ParenthesesPair()
		{
			// Arrange
			const string input = "()";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(PunctuatorToken),
				typeof(PunctuatorToken));
		}


		[TestMethod]
		public void Tokenize_PunctuatorsBundle()
		{
			// Arrange
			const string input = "(;,{)}]};[[";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			CollectionAssert.That.TokensAreSameType(result, typeof(PunctuatorToken));
			Assert.AreEqual(input.Length, result.Length, "Wrong token count in result.");
		}
	}
}