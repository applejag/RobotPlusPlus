using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleFunctionCallTests
	{
		[TestMethod]
		public void Parse_FunctionCall_0Param()
		{
			// Arrange
			const string code = "func()";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token func = result[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "(");
			Assert.That.TokenIsOfType<IdentifierToken>(func[0], "func");
			Assert.That.TokenIsParentases(func[1], '(', 0);
		}

		[TestMethod]
		public void Parse_FunctionCall_1Param()
		{
			// Arrange
			const string code = "func('foo')";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token func = result[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "(");
			Assert.That.TokenIsOfType<IdentifierToken>(func[0], "func");

			Token par = func[1];
			Assert.That.TokenIsParentases(par, '(', 1);
			Assert.That.TokenIsLiteralString(par[0], "foo");
		}

		[TestMethod]
		public void Parse_FunctionCall_2Param()
		{
			// Arrange
			const string code = "func('foo', 2)";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token func = result[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "(");
			Assert.That.TokenIsOfType<IdentifierToken>(func[0], "func");

			Token par = func[1];
			Assert.That.TokenIsParentases(par, '(', 3);
			Assert.That.TokenIsLiteralString(par[0], "foo");
			Assert.That.TokenIsOfType<PunctuatorToken>(par[1], ",");
			Assert.That.TokenIsLiteralInteger(par[2], 2);
		}

		[TestMethod]
		public void Parse_FunctionOnIdentifierCall_0Param()
		{
			// Arrange
			const string code = "x.func()";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token func = result[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "(");

			Token dot = func[0];
			Assert.That.TokenIsDotOperation(dot);
			Assert.That.TokenIsOfType<IdentifierToken>(dot[0], "x");
			Assert.That.TokenIsOfType<IdentifierToken>(dot[1], "func");

			Token par = func[1];
			Assert.That.TokenIsParentases(par, '(', 0);
		}

		[TestMethod]
		public void Parse_FunctionOnIdentifierCall_1Param()
		{
			// Arrange
			const string code = "x.func(50)";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token func = result[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "(");

			Token dot = func[0];
			Assert.That.TokenIsDotOperation(dot);
			Assert.That.TokenIsOfType<IdentifierToken>(dot[0], "x");
			Assert.That.TokenIsOfType<IdentifierToken>(dot[1], "func");

			Token par = func[1];
			Assert.That.TokenIsParentases(par, '(', 1);
			Assert.That.TokenIsLiteralInteger(par[0], 50);
		}

		[TestMethod]
		public void Parse_FunctionOnLiteralCall_1Param()
		{
			// Arrange
			const string code = "'lorem'.func(50)";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token func = result[0];
			Assert.That.TokenIsOfType<FunctionCallToken>(func, "(");

			Token dot = func[0];
			Assert.That.TokenIsDotOperation(dot);
			Assert.That.TokenIsLiteralString(dot[0], "lorem");
			Assert.That.TokenIsOfType<IdentifierToken>(dot[1], "func");

			Token par = func[1];
			Assert.That.TokenIsParentases(par, '(', 1);
			Assert.That.TokenIsLiteralInteger(par[0], 50);
		}
	}
}