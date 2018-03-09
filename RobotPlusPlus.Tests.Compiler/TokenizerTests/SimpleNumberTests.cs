using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
	public class SimpleNumberTests
	{
		[TestMethod]
		public void Tokenize_OneIntegerSingleDigit()
		{
			// Arrange
			const string input = @"1";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneIntegerMultipleDigit()
		{
			// Arrange
			const string input = @"2147483647";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneReal()
		{
			// Arrange
			const string input = @"1.0";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneRealNothingAfterPoint()
		{
			// Arrange
			const string input = @"1.";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneRealNothingBeforePoint()
		{
			// Arrange
			const string input = @".005";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneRealMultipleDigits()
		{
			// Arrange
			const string input = @"9000.0001";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneIntegerNegative()
		{
			// Arrange
			const string input = @"-666";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Operator,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneRealNegative()
		{
			// Arrange
			const string input = @"-13.37";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Operator,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneRealNegativeNothingBeforePoint()
		{
			// Arrange
			const string input = @"-.5";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Operator,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneRealNegativeNothingAfterPoint()
		{
			// Arrange
			const string input = @"-10.";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Operator,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

	}
}