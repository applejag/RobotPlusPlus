using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleIfStatementTests
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_IfNoCondition()
		{
			// Arrange
			const string code = "if {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Parse_IfNoBlock()
		{
			// Arrange
			const string code = "if 1 > 0";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Parse_IfCodeBlockEmpty()
		{
			// Arrange
			const string code = "if true {}";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);
		}

		[TestMethod]
		public void Parse_IfCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "if true { x = 1 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 1);

			Token x = _if[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
		}

		[TestMethod]
		public void Parse_IfCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "if true { x = 1 z = x }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 2);

			Token x = _if[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
			
			Token z = _if[1][1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsOfType<IdentifierToken>(z[1], "x");
		}

		[TestMethod]
		public void Parse_IfSingleStatement()
		{
			// Arrange
			const string code = "if true x = 2";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);

			Token x = _if[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Parse_IfOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "if true x = 2 z = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);

			Token x = _if[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);

			Token z = result[1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsLiteralInteger(z[1], 10);
		}

		[TestMethod]
		public void Parse_IfNestedBlocks()
		{
			// Arrange
			const string code = "if true { if false { a = 0 } }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 1);

			Token _if2 = _if[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token a = _if2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Parse_IfNestedFirstHasBlock()
		{
			// Arrange
			const string code = "if true { if false a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 1);

			Token _if2 = _if[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);

			Token a = _if2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Parse_IfNestedSecondHasBlock()
		{
			// Arrange
			const string code = "if true if false { a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);

			Token _if2 = _if[1];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token a = _if2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}


		[TestMethod]
		public void Parse_IfEmbeddedAssignment()
		{
			// Arrange
			const string code = "if (x = 2) > 1 { }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token gt = _if[0];
			Assert.That.TokenIsOperator(gt, OperatorToken.Type.Relational, ">");
			Assert.That.TokenIsParentases(gt[0], '(', 1);
			Assert.That.TokenIsLiteralInteger(gt[1], 1);

			Token x = gt[0][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Parse_IfMultipleNestedBlocks()
		{
			// Arrange
			const string code = "if true { if false { a = 0 } if 99 { b = 98 } }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 2);

			Token _if2 = _if[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token a = _if2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _if3 = _if[1][1];
			Assert.That.TokenIsOfType<StatementToken>(_if3, "if");
			Assert.That.TokenIsLiteralInteger(_if3[0], 99);
			Assert.That.TokenIsParentases(_if3[1], '{', 1);

			Token b = _if3[1][0];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

		[TestMethod]
		public void Parse_IfMultipleNestedNoBlocks()
		{
			// Arrange
			const string code = "if true if false a = 0 if 99 b = 98";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);

			Token _if2 = _if[1];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);

			Token a = _if2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _if3 = result[1];
			Assert.That.TokenIsOfType<StatementToken>(_if3, "if");
			Assert.That.TokenIsLiteralInteger(_if3[0], 99);

			Token b = _if3[1];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

		[TestMethod]
		public void Parse_IfMultipleNestedTopBlocks()
		{
			// Arrange
			const string code = "if true { if false a = 0 if 99 b = 98 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 2);

			Token _if2 = _if[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);

			Token a = _if2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);

			Token _if3 = _if[1][1];
			Assert.That.TokenIsOfType<StatementToken>(_if3, "if");
			Assert.That.TokenIsLiteralInteger(_if3[0], 99);

			Token b = _if3[1];
			Assert.That.TokenIsOperator(b, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(b[0], "b");
			Assert.That.TokenIsLiteralInteger(b[1], 98);
		}

	}
}