using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleDoWhileStatementTests
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_DoWhileNoCondition()
		{
			// Arrange
			const string code = "do {} while";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_DoWhileNoBlock()
		{
			// Arrange
			const string code = "do while 1 > 0";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Parse_DoWhileCodeBlockEmpty()
		{
			// Arrange
			const string code = "do {} while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 0);
		}

		[TestMethod]
		public void Parse_DoWhileCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "do { x = 1 } while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 1);

			Token x = _do[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
		}

		[TestMethod]
		public void Parse_DoWhileCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "do { x = 1 z = x } while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 2);

			Token x = _do[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
			
			Token z = _do[1][1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsOfType<IdentifierToken>(z[1], "x");
		}

		[TestMethod]
		public void Parse_DoWhileSingleStatement()
		{
			// Arrange
			const string code = "do x = 2 while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);

			Token x = _do[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_DoWhileExpectedWhile()
		{
			// Arrange
			const string code = "do x = 2 z = 10 while true";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Parse_DoWhileOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "do x = 2 while true z = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);

			Token x = _do[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);

			Token z = result[1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsLiteralInteger(z[1], 10);
		}

		[TestMethod]
		public void Parse_DoWhileNestedBlocks()
		{
			// Arrange
			const string code = "do { do { a = 0 } while false } while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 1);

			Token _do2 = _do[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_do2, "do");
			Assert.That.TokenIsLiteralKeyword(_do2[0], false);
			Assert.That.TokenIsParentases(_do2[1], '{', 1);

			Token a = _do2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Parse_DoWhileNestedFirstHasBlock()
		{
			// Arrange
			const string code = "do { do a = 0 while false } while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 1);

			Token _do2 = _do[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_do2, "do");
			Assert.That.TokenIsLiteralKeyword(_do2[0], false);

			Token a = _do2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Parse_DoWhileNestedSecondHasBlock()
		{
			// Arrange
			const string code = "do do { a = 0 } while false while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);

			Token _do2 = _do[1];
			Assert.That.TokenIsOfType<StatementToken>(_do2, "do");
			Assert.That.TokenIsLiteralKeyword(_do2[0], false);
			Assert.That.TokenIsParentases(_do2[1], '{', 1);

			Token a = _do2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}


		[TestMethod]
		public void Parse_DoWhileEmbeddedAssignment()
		{
			// Arrange
			const string code = "do { } while (x = 2) > 1";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsParentases(_do[1], '{', 0);

			Token gt = _do[0];
			Assert.That.TokenIsOperator(gt, OperatorToken.Type.Relational, ">");
			Assert.That.TokenIsParentases(gt[0], '(', 1);
			Assert.That.TokenIsLiteralInteger(gt[1], 1);

			Token x = gt[0][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Parse_DoWhileMultipleNestedBlocks()
		{
			// Arrange
			const string code = "do { do { a = 0 } while false do { b = 98 } while 99 } while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 2);

			Token _do2 = _do[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_do2, "do");
			Assert.That.TokenIsLiteralKeyword(_do2[0], false);
			Assert.That.TokenIsParentases(_do2[1], '{', 1);

			Token a = _do2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _do3 = _do[1][1];
			Assert.That.TokenIsOfType<StatementToken>(_do3, "do");
			Assert.That.TokenIsLiteralInteger(_do3[0], 99);
			Assert.That.TokenIsParentases(_do3[1], '{', 1);

			Token b = _do3[1][0];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

		[TestMethod]
		public void Parse_DoWhileMultipleNestedNoBlocks()
		{
			// Arrange
			const string code = "do do a = 0 while false while true do b = 98 while 99";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);

			Token _do2 = _do[1];
			Assert.That.TokenIsOfType<StatementToken>(_do2, "do");
			Assert.That.TokenIsLiteralKeyword(_do2[0], false);

			Token a = _do2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _do3 = result[1];
			Assert.That.TokenIsOfType<StatementToken>(_do3, "do");
			Assert.That.TokenIsLiteralInteger(_do3[0], 99);

			Token b = _do3[1];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

		[TestMethod]
		public void Parse_DoWhileMultipleNestedTopBlocks()
		{
			// Arrange
			const string code = "do { do a = 0 while false do b = 98 while 99 } while true";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _do = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_do, "do");
			Assert.That.TokenIsLiteralKeyword(_do[0], true);
			Assert.That.TokenIsParentases(_do[1], '{', 2);

			Token _do2 = _do[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_do2, "do");
			Assert.That.TokenIsLiteralKeyword(_do2[0], false);

			Token a = _do2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _do3 = _do[1][1];
			Assert.That.TokenIsOfType<StatementToken>(_do3, "do");
			Assert.That.TokenIsLiteralInteger(_do3[0], 99);

			Token b = _do3[1];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

	}
}