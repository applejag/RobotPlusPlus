using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Tests.TokenizerTests
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

	}
}