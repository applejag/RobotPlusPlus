using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Linguist.Tokenizing;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;
using RobotPlusPlus.Linguist.Tokenizing.Tokens.Literals;

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
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(LiteralKeyword));
		}

		[TestMethod]
		public void Tokenize_LiteralFalse()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("false");

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(LiteralKeyword));
		}

		[TestMethod]
		public void Tokenize_LiteralNull()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("null");

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(LiteralKeyword));
		}
		
		[TestMethod]
		public void Tokenize_LiteralTrueNonLowercase()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("trUe");

			// Assert
			Assert.IsNotNull(result, "Tokens list is null.");
			Assert.IsNotNull(result[0], "tokens[0] is null.");
			Assert.IsNotInstanceOfType(result[0], typeof(LiteralKeyword));
		}
	}
}