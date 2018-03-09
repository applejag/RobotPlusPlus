using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
	public class SimpleStringTests
	{
		[TestMethod]
		public void Tokenize_OneDoubleQuoteString()
		{
			// Arrange
			const string input = @"""hello world""";
			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_OneSingleQuoteString()
		{
			// Arrange
			const string input = @"'hello world'";
			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseException))]
		public void Tokenize_IncompleteDoubleQuoteString()
		{
			// Arrange
			const string input = @"""hello world";
			// Act
			Tokenizer.Tokenize(input);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(ParseException))]
		public void Tokenize_IncompleteSingleQuoteString()
		{
			// Arrange
			const string input = @"'hello world";
			// Act
			Tokenizer.Tokenize(input);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void Tokenize_NestedSingleInDoubleQuoteString()
		{
			// Arrange
			const string input = @"""nested 'strings' are cool""";
			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_NestedDoubleInSingleQuoteString()
		{
			// Arrange
			const string input = @"'I dont use ""airquotes"" correctly'";
			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}

		[TestMethod]
		public void Tokenize_EscapedString()
		{
			// Arrange
			const string input = @"'You\'re fine'";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);

			Assert.AreEqual(input, result[0].Source);
		}
	}
}