using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

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
				typeof(Punctuator));
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
				typeof(Punctuator),
				typeof(Punctuator));
		}


		[TestMethod]
		public void Tokenize_PunctuatorsBundle()
		{
			// Arrange
			const string input = "(;:,:.{)}]}:;[[.";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypesAllSame(result, typeof(Punctuator));
			Assert.AreEqual(input.Length, result.Length, "Wrong token count in result.");
		}
	}
}