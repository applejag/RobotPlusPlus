using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Tests.TokenizerTests
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
			Utility.AssertTokenTypes(result,
				TokenType.Punctuators);
		}

		[TestMethod]
		public void Tokenize_ParenthesesPair()
		{
			// Arrange
			const string input = "()";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Punctuators,
				TokenType.Punctuators);
		}


		[TestMethod]
		public void Tokenize_PunctuatorsBundle()
		{
			// Arrange
			const string input = "(;:,:.{)}]}:;[[.";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypesAllSame(result, TokenType.Punctuators);
			Assert.AreEqual(input.Length, result.Length, "Wrong token count in result.");
		}
	}
}