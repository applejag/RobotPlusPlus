using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.TokenizerTests
{
	[TestClass]
	public class SimpleTests
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
			Token[] result = Tokenizer.Tokenize("   \t\t  \r\n \r\t  \n");

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(Whitespace));
		}

		[TestMethod]
		public void Tokenize_Newlines()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("\n\n\n");

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(Whitespace));

			Assert.That.TokenIsOfType<Whitespace>(result[0], "\n\n\n");
			Assert.AreEqual(3, result[0].NewLines);
		}

		[TestMethod]
		public void Tokenize_NewlinesWithSep()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("\n\n\n;\n\n\n\n");

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(Whitespace),
				typeof(Punctuator),
				typeof(Whitespace));

			Assert.That.TokenIsOfType<Whitespace>(result[0], "\n\n\n");
			Assert.AreEqual(3, result[0].NewLines);
			Assert.That.TokenIsOfType<Punctuator>(result[1], ";");
			Assert.That.TokenIsOfType<Whitespace>(result[2], "\n\n\n\n");
			Assert.AreEqual(4, result[2].NewLines);
		}

	}
}