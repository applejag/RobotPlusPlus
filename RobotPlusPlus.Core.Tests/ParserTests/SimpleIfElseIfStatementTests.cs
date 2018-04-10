using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class SimpleIfElseIfStatementTests
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseIf_NoCondition_If()
		{
			// Arrange
			const string code = "if {} else if true {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseIf_NoCondition_ElseIf()
		{
			// Arrange
			const string code = "if true {} else if {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseIf_NoBlock_If()
		{
			// Arrange
			const string code = "if 1 > 0 else if {}";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfElseIf_NoBlock_ElseIf()
		{
			// Arrange
			const string code = "if 1 > 0 {} else if";

			// Act
			Parser.Parse(Tokenizer.Tokenize(code));
		}

		[TestMethod]
		public void Compile_IfElseIf_CodeBlockEmpty()
		{
			// Arrange
			const string code = "if true {} else if false {}";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 0);
		}

		[TestMethod]
		public void Compile_IfElseIf_CodeBlockSingleStatement_If()
		{
			// Arrange
			const string code = "if true { x = 1 } else if false {}";

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

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 0);
		}

		[TestMethod]
		public void Compile_IfElseIf_CodeBlockSingleStatement_ElseIf()
		{
			// Arrange
			const string code = "if true { } else if false { x = 1 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token x = _if2[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
		}

		[TestMethod]
		public void Compile_IfElseIf_CodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "if true { } else if false { x = 1 z = x }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 2);

			Token x = _if2[1][0];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 1);
			
			Token z = _if2[1][1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsOfType<IdentifierToken>(z[1], "x");
		}

		[TestMethod]
		public void Compile_IfElseIf_SingleStatement()
		{
			// Arrange
			const string code = "if true {} else if false x = 2";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);

			Token x = _if2[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);
		}

		[TestMethod]
		public void Compile_IfElseIf_OneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "if true {} else if false x = 2 z = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(2, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);

			Token x = _if2[1];
			Assert.That.TokenIsOperator(x, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(x[0], "x");
			Assert.That.TokenIsLiteralInteger(x[1], 2);

			Token z = result[1];
			Assert.That.TokenIsOperator(z, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(z[0], "z");
			Assert.That.TokenIsLiteralInteger(z[1], 10);
		}

		[TestMethod]
		public void Compile_IfElseIf_NestedBlocks()
		{
			// Arrange
			const string code = "if true {} else if false { if 2 { a = 0 } }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);
			
			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralKeyword(_if2[0], false);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token _if3 = _if2[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_if3, "if");
			Assert.That.TokenIsLiteralInteger(_if3[0], 2);
			Assert.That.TokenIsParentases(_if3[1], '{', 1);

			Token a = _if3[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Compile_IfElseIf_NestedFirstHasBlock()
		{
			// Arrange
			const string code = "if true {} else if 2 { if false a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);
			
			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralInteger(_if2[0], 2);
			Assert.That.TokenIsParentases(_if2[1], '{', 1);

			Token _if3 = _if2[1][0];
			Assert.That.TokenIsOfType<StatementToken>(_if3, "if");
			Assert.That.TokenIsLiteralKeyword(_if3[0], false);

			Token a = _if3[1];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

		[TestMethod]
		public void Compile_IfElseIf_NestedSecondHasBlock()
		{
			// Arrange
			const string code = "if true {} else if 2 if false { a = 0 }";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);
			Assert.AreEqual(1, result.Length);

			Token _if = result[0];
			Assert.That.TokenIsOfType<StatementToken>(_if, "if");
			Assert.That.TokenIsLiteralKeyword(_if[0], true);
			Assert.That.TokenIsParentases(_if[1], '{', 0);

			Token _if2 = _if[2];
			Assert.That.TokenIsOfType<StatementToken>(_if2, "if");
			Assert.That.TokenIsLiteralInteger(_if2[0], 2);

			Token _if3 = _if2[1];
			Assert.That.TokenIsOfType<StatementToken>(_if3, "if");
			Assert.That.TokenIsLiteralKeyword(_if3[0], false);
			Assert.That.TokenIsParentases(_if3[1], '{', 1);

			Token a = _if3[1][0];
			Assert.That.TokenIsOperator(a, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(a[0], "a");
			Assert.That.TokenIsLiteralInteger(a[1], 0);
		}

	}
}