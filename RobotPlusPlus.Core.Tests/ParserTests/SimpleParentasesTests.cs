using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleParentasesTests
	{
		[TestMethod]
		public void Parse_ParentasesAroundLiteral()
		{
			// Arrange
			const string code = "(5)";

			// Act
			Token[] parsed = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(parsed);
			Assert.AreEqual(1, parsed.Length);
			Assert.That.TokenIsLiteralInteger(parsed[0], 5);
		}

		[TestMethod]
		public void Parse_DoubleParentasesAroundLiteral()
		{
			// Arrange
			const string code = "((10))";

			// Act
			Token[] parsed = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(parsed);
			Assert.AreEqual(1, parsed.Length);
			Assert.That.TokenIsLiteralInteger(parsed[0], 10);
		}

		[TestMethod]
		public void Parse_ParentasesAroundOperation()
		{
			// Arrange
			const string code = "(1 + 2)";

			// Act
			Token[] parsed = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(parsed);
			Assert.AreEqual(1, parsed.Length);
			Assert.That.TokenIsOperator(parsed[0], OperatorToken.Type.Additive, "+");
			Assert.That.TokenIsLiteralInteger(parsed[0][0], 1);
			Assert.That.TokenIsLiteralInteger(parsed[0][1], 2);
		}

		[TestMethod]
		public void Parse_DoubleParentasesAroundOperation()
		{
			// Arrange
			const string code = "((1) + (2))";

			// Act
			Token[] parsed = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(parsed);
			Assert.AreEqual(1, parsed.Length);
			Assert.That.TokenIsOperator(parsed[0], OperatorToken.Type.Additive, "+");
			Assert.That.TokenIsLiteralInteger(parsed[0][0], 1);
			Assert.That.TokenIsLiteralInteger(parsed[0][1], 2);
		}
		
	}
}