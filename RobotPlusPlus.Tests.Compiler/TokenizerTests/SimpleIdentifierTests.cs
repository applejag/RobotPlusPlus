using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;
using System.Collections.Generic;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
    public class SimpleIdentifierTests
    {
		private static void AssertSingleIdentifier(string input, IReadOnlyList<Token> result)
		{
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);

			Assert.IsNotNull(result[0]);
			Assert.AreEqual(result[0].Type, TokenType.Identifier);
			Assert.AreEqual(input.Trim(), result[0].Source);
		}

		[TestMethod]
		public void Tokenize_SingleIdentifierSingleChar()
		{
			// Arrange
			const string input = "i";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			AssertSingleIdentifier(input, result);
		}

		[TestMethod]
		public void Tokenize_SingleIdentifierMultipleChar()
		{
			// Arrange
			const string input = "myVariable";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			AssertSingleIdentifier(input, result);
		}

		[TestMethod]
		public void Tokenize_SingleIdentifierMultipleCharWithSpaces()
		{
			// Arrange
			const string input = "	 myVariable		";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			AssertSingleIdentifier(input, result);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseException))]
		public void Tokenize_SingleIdentifierWithInvalidFirstChar()
		{
			// Arrange
			const string input = "10dogs";

			// Act
			Tokenizer.Tokenize(input);

			Assert.Fail();
		}

		[TestMethod]
		public void Tokenize_MultipleIdentifiersSimple()
		{
			// Arrange
			const string input = "my varIab13s shine";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);

			Assert.IsNotNull(result[0]);
			Assert.AreEqual(result[0].Type, TokenType.Identifier);
			Assert.AreEqual("my", result[0].Source);

			Assert.IsNotNull(result[1]);
			Assert.AreEqual(result[1].Type, TokenType.Identifier);
			Assert.AreEqual("varIab13s", result[1].Source);

			Assert.IsNotNull(result[2]);
			Assert.AreEqual(result[2].Type, TokenType.Identifier);
			Assert.AreEqual("shine", result[2].Source);
		}
	}
}
