using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleDotOperatorTests
	{
		[TestMethod]
		public void Parse_DotAccessVariable()
		{
			// Arrange
			const string code = "x.y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][0], "x");
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessLiteralInteger()
		{
			// Arrange
			const string code = "1.y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsLiteralInteger(result[0][0], 1);
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessLiteralReal()
		{
			// Arrange
			const string code = "1..y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsLiteralReal(result[0][0], 1);
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessLiteralRealF()
		{
			// Arrange
			const string code = "1f.y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsLiteralReal(result[0][0], 1);
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessLiteralReal0()
		{
			// Arrange
			const string code = "1.0.y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsLiteralReal(result[0][0], 1);
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessLiteralString()
		{
			// Arrange
			const string code = "\"foo\".y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsLiteralString(result[0][0], "foo");
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessLiteralKeyword()
		{
			// Arrange
			const string code = "true.y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsDotOperation(result[0]);
			Assert.That.TokenIsOfType<LiteralKeywordToken>(result[0][0], "true");
			Assert.That.TokenIsOfType<IdentifierToken>(result[0][1], "y");
		}

		[TestMethod]
		public void Parse_DotAccessFunctionCall()
		{
			// Arrange
			const string code = "x().y";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token dot = result[0];
			Assert.That.TokenIsDotOperation(dot);
			Token func = dot[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "x()");
			Assert.That.TokenIsOfType<IdentifierToken>(func[0], "x");
			Assert.That.TokenIsParentases(func[1], '(', 0);
			Assert.That.TokenIsOfType<IdentifierToken>(dot[1], "y");
		}
	}
}