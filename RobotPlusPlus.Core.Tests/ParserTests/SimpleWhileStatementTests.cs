using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleWhileStatementTests
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_WhileNoCondition()
		{
			// Arrange
			const string code = "while {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_WhileNoBlock()
		{
			// Arrange
			const string code = "while 1 > 0";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Parse_WhileCodeBlockEmpty()
		{
			// Arrange
			const string code = "while true {}";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 0);
		}

		[TestMethod]
		public void Parse_WhileCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "while true { x = 1 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 1);

			Token x = _while[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
		}

		[TestMethod]
		public void Parse_WhileCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "while true { x = 1 z = x }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 2);

			Token x = _while[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
			
			Token z = _while[1][1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsOfType<IdentifierToken>(z[1], "x");
		}

		[TestMethod]
		public void Parse_WhileSingleStatement()
		{
			// Arrange
			const string code = "while true x = 2";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);

			Token x = _while[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Parse_WhileOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "while true x = 2 z = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);

			Token x = _while[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);

			Token z = result[1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsLiteralInteger(z[1], 10);
		}

		[TestMethod]
		public void Parse_WhileNestedBlocks()
		{
			// Arrange
			const string code = "while true { while false { a = 0 } }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 1);

			Token _while2 = _while[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_while2, "while");
			Assert.That.TokenIsLiteralKeyword(_while2[0], false);
			Assert.That.TokenIsParentases(_while2[1], '{', 1);

			Token a = _while2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Parse_WhileNestedFirstHasBlock()
		{
			// Arrange
			const string code = "while true { while false a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 1);

			Token _while2 = _while[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_while2, "while");
			Assert.That.TokenIsLiteralKeyword(_while2[0], false);

			Token a = _while2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Parse_WhileNestedSecondHasBlock()
		{
			// Arrange
			const string code = "while true while false { a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);

			Token _while2 = _while[1];
			Assert.That.TokenIsOfType<StatementToken>(_while2, "while");
			Assert.That.TokenIsLiteralKeyword(_while2[0], false);
			Assert.That.TokenIsParentases(_while2[1], '{', 1);

			Token a = _while2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}


		[TestMethod]
		public void Parse_WhileEmbeddedAssignment()
		{
			// Arrange
			const string code = "while (x = 2) > 1 { }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsParentases(_while[1], '{', 0);

			Token gt = _while[0];
			Assert.That.TokenIsOperator(gt, OperatorToken.Type.Relational, ">");
			Assert.That.TokenIsParentases(gt[0], '(', 1);
			Assert.That.TokenIsLiteralInteger(gt[1], 1);

			Token x = gt[0][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Parse_WhileMultipleNestedBlocks()
		{
			// Arrange
			const string code = "while true { while false { a = 0 } while 99 { b = 98 } }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 2);

			Token _while2 = _while[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_while2, "while");
			Assert.That.TokenIsLiteralKeyword(_while2[0], false);
			Assert.That.TokenIsParentases(_while2[1], '{', 1);

			Token a = _while2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _while3 = _while[1][1];
			Assert.That.TokenIsOfType<StatementToken>(_while3, "while");
			Assert.That.TokenIsLiteralInteger(_while3[0], 99);
			Assert.That.TokenIsParentases(_while3[1], '{', 1);

			Token b = _while3[1][0];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

		[TestMethod]
		public void Parse_WhileMultipleNestedNoBlocks()
		{
			// Arrange
			const string code = "while true while false a = 0 while 99 b = 98";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);

			Token _while2 = _while[1];
			Assert.That.TokenIsOfType<StatementToken>(_while2, "while");
			Assert.That.TokenIsLiteralKeyword(_while2[0], false);

			Token a = _while2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _while3 = result[1];
			Assert.That.TokenIsOfType<StatementToken>(_while3, "while");
			Assert.That.TokenIsLiteralInteger(_while3[0], 99);

			Token b = _while3[1];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

		[TestMethod]
		public void Parse_WhileMultipleNestedTopBlocks()
		{
			// Arrange
			const string code = "while true { while false a = 0 while 99 b = 98 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _while = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_while, "while");
			Assert.That.TokenIsLiteralKeyword(_while[0], true);
			Assert.That.TokenIsParentases(_while[1], '{', 2);

			Token _while2 = _while[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_while2, "while");
			Assert.That.TokenIsLiteralKeyword(_while2[0], false);

			Token a = _while2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _while3 = _while[1][1];
			Assert.That.TokenIsOfType<StatementToken>(_while3, "while");
			Assert.That.TokenIsLiteralInteger(_while3[0], 99);

			Token b = _while3[1];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

	}
}