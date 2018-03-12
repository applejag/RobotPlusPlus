using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
	public class SimpleLiteralKeywordTests
	{
		[TestMethod]
		public void Tokenize_LiteralTrue()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("true");

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);
		}

		[TestMethod]
		public void Tokenize_LiteralFalse()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("false");

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);
		}

		[TestMethod]
		public void Tokenize_LiteralNull()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("null");

			// Assert
			Utility.AssertTokenTypes(result,
				TokenType.Literal);
		}
		
		[TestMethod]
		public void Tokenize_LiteralTrueNonLowercase()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("trUe");

			// Assert
			Assert.IsNotNull(result, "Tokens list is null.");
			Assert.IsNotNull(result[0], "tokens[0] is null.");
			Assert.AreNotEqual(TokenType.Literal, result[0]);
		}
	}
}