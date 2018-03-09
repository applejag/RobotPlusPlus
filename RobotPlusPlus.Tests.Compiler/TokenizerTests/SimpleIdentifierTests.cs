using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;
using System.Collections.Generic;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
    public class SimpleIdentifierTests
    {

		[TestMethod]
		public void Tokenize_SingleIdentifierSingleChar()
		{
			// Arrange
			const string input = "i";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Identifier);
		}

		[TestMethod]
		public void Tokenize_SingleIdentifierMultipleChar()
		{
			// Arrange
			const string input = "myVariable";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Identifier);
		}

		[TestMethod]
		public void Tokenize_SingleIdentifierMultipleCharWithSpaces()
		{
			// Arrange
			const string input = "	 myVariable		";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Whitespace,
				TokenType.Identifier,
				TokenType.Whitespace);
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
			Utility.AssertTokenTypes(result,
				TokenType.Identifier,
				TokenType.Whitespace,
				TokenType.Identifier,
				TokenType.Whitespace,
				TokenType.Identifier);
		}
	}
}
