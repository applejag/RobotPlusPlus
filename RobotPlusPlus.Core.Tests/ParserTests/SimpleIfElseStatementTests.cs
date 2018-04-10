using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleIfElseStatementTests
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseNoCondition()
		{
			// Arrange
			const string code = "if {} else {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseNoBlock()
		{
			// Arrange
			const string code = "if 1 > 0 else {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseElseNoBlock()
		{
			// Arrange
			const string code = "if 1 > 0 {} else";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Compile_IfElseCodeBlockEmpty()
		{
			// Arrange
			const string code = "if true {} else {}";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _else = _if[2];
			Assert.That.TokenIsParentases(_else, '{', 0);
		}

		[TestMethod]
		public void Compile_IfElseCodeBlockIfSingleStatement()
		{
			// Arrange
			const string code = "if true { x = 1 } else {}";

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

			Token _else = _if[2];
			Assert.That.TokenIsParentases(_else, '{', 0);
		}

		[TestMethod]
		public void Compile_IfElseCodeBlockElseSingleStatement()
		{
			// Arrange
			const string code = "if true { } else { x = 1 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _else = _if[2];
			Assert.That.TokenIsParentases(_else, '{', 1);

			Token x = _else[0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
		}

		[TestMethod]
		public void Compile_IfElseCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "if true { } else { x = 1 z = x }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);
			
			Token _else = _if[2];
			Assert.That.TokenIsParentases(_else, '{', 2);

			Token x = _else[0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
			
			Token z = _else[1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsOfType<IdentifierToken>(z[1], "x");
		}

		[TestMethod]
		public void Compile_IfElseSingleStatement()
		{
			// Arrange
			const string code = "if true {} else x = 2";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token x = _if[2];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Compile_IfElseOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "if true {} else x = 2 z = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token x = _if[2];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);

			Token z = result[1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsLiteralInteger(z[1], 10);
		}

		[TestMethod]
		public void Compile_IfElseNestedBlocks()
		{
			// Arrange
			const string code = "if true {} else { if false { a = 0 } }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _else = _if[2];
			Assert.That.TokenIsParentases(_else, '{', 1);

			Token _if2 = _else[0];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token a = _if2[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Compile_IfElseNestedFirstHasBlock()
		{
			// Arrange
			const string code = "if true {} else { if false a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _else = _if[2];
			Assert.That.TokenIsParentases(_else, '{', 1);

			Token _if2 = _else[0];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);

			Token a = _if2[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

	}
}