using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Tests
{
	[TestClass]
	public class TokenizerSimpleTests
	{
		[TestMethod]
		public void Tokenize_Null()
		{
			// Act
			Token[] result = Tokenizer.Tokenize(null);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void Tokenize_Whitespace()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("   	 		  ");

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);
		}

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