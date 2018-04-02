using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleUnaryTests
	{
		[TestMethod]
		public void Parse_NumberNegative()
		{
			// Arrange
			const string code = "-50";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsOperator(result[0], OperatorToken.Type.Unary, "-");
			Assert.That.TokenIsLiteralInteger(result[0][1], 50);
		}

		[TestMethod]
		public void Parse_NumberNegativeNegative()
		{
			// Arrange
			const string code = "- -50";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsOperator(result[0], OperatorToken.Type.Unary, "-");
			Assert.That.TokenIsOperator(result[0][0], OperatorToken.Type.Unary, "-");
			Assert.That.TokenIsLiteralInteger(result[0][0][1], 50);
		}

		[TestMethod]
		public void Parse_NumberPositive()
		{
			// Arrange
			const string code = "+50";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsOperator(result[0], OperatorToken.Type.Unary, "+");
			Assert.That.TokenIsLiteralInteger(result[0][1], 50);
		}

		[TestMethod]
		public void Parse_ParentasesNegative()
		{
			// Arrange
			const string code = "-(50 + 5)";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Assert.That.TokenIsOperator(result[0], OperatorToken.Type.Unary, "-");

			Token par = result[0][1];
			Assert.That.TokenIsParentases(par, '(', 1);
			Token add = par[0];
			Assert.That.TokenIsLiteralInteger(add[0], 50);
			Assert.That.TokenIsLiteralInteger(add[1], 5);
		}

		[TestMethod]
		public void Parse_InExpression()
		{
			// Arrange
			const string code = "50 + -5";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token add = result[0];
			Assert.That.TokenIsLiteralInteger(add[0], 50);

			Token unary = add[1];
			Assert.That.TokenIsOperator(unary, OperatorToken.Type.Unary, "-");
			Assert.That.TokenIsLiteralInteger(unary[1], 5);
		}

		[TestMethod]
		public void Parse_NegateBoolean()
		{
			// Arrange
			const string code = "!true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token unary = result[0];
			Assert.That.TokenIsOperator(unary, OperatorToken.Type.Unary, "!");
			Assert.That.TokenIsLiteralKeyword(unary[1], true);
		}

		[TestMethod]
		public void Parse_NegateNegateBoolean()
		{
			// Arrange
			const string code = "!!true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token unary1 = result[0];
			Assert.That.TokenIsOperator(unary1, OperatorToken.Type.Unary, "!");
			Token unary2 = unary1[1];
			Assert.That.TokenIsOperator(unary2, OperatorToken.Type.Unary, "!");
			Assert.That.TokenIsLiteralKeyword(unary2[1], true);
		}

		[TestMethod]
		public void Parse_TrippleNegateVariable()
		{
			// Arrange
			const string code = "---x";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token exp = result[0];
			Assert.That.TokenIsOperator(exp, OperatorToken.Type.Expression, "++");
			Assert.That.TokenIsOfType<IdentifierToken>(exp[1], "x");
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_PrefixNumber()
		{
			// Arrange
			const string code = "--5";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_PrefixParentases()
		{
			// Arrange
			const string code = "--(x)";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Parse_PrefixVariable()
		{
			// Arrange
			const string code = "++x";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token exp = result[0];
			Assert.That.TokenIsOperator(exp, OperatorToken.Type.Expression, "++");
			Assert.That.TokenIsOfType<IdentifierToken>(exp[1], "x");
		}

		[TestMethod]
		public void Parse_PostfixVariable()
		{
			// Arrange
			const string code = "x++";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token exp = result[0];
			Assert.That.TokenIsOperator(exp, OperatorToken.Type.Expression, "++");
			Assert.That.TokenIsOfType<IdentifierToken>(exp[0], "x");
		}

		[TestMethod]
		public void Parse_PostfixNegativeVariable()
		{
			// Arrange
			const string code = "-x++";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token unary = result[0];
			Assert.That.TokenIsOperator(unary, OperatorToken.Type.Unary, "-");
			Token exp = unary[1];
			Assert.That.TokenIsOperator(exp, OperatorToken.Type.Expression, "++");
			Assert.That.TokenIsOfType<IdentifierToken>(exp[0], "x");
		}

		[TestMethod]
		public void Parse_PrefixNegativeVariable()
		{
			// Arrange
			const string code = "-++x";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token unary = result[0];
			Assert.That.TokenIsOperator(unary, OperatorToken.Type.Unary, "-");
			Token exp = unary[1];
			Assert.That.TokenIsOperator(exp, OperatorToken.Type.Expression, "++");
			Assert.That.TokenIsOfType<IdentifierToken>(exp[1], "x");
		}

		[TestMethod]
		// Should parse as ++(x++), therefore expected TRAILING
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_PrePostfixVariable()
		{
			// Arrange
			const string code = "++x++";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}
	}
}