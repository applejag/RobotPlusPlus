using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Tests.ParserTests
{
	[TestClass]
	public class IntermediateAssignmentTests
	{

		[TestMethod]
		public void Parse_NestedAssignment()
		{
			// Arrange
			const string code = "y = x = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);

			Token yassign = result[0];
			Assert.That.TokenIsOperator(yassign, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(yassign[0], "y");

			Token xassign = yassign[1];
			Assert.That.TokenIsOperator(xassign, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(xassign[0], "x");
			Assert.That.TokenIsLiteralInteger(xassign[1], 10);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedLeadingTokenException))]
		public void Parse_NestedAssignmentParentasesInvalid()
		{
			// Arrange
			const string code = "y = (x) = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedLeadingTokenException))]
		public void Parse_NestedAssignmentPriorityInvalid()
		{
			// Arrange
			// Should compile into (y = ((5 + x) = 10)), which is invalid
			const string code = "y = 5 + x = 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
		}

		[TestMethod]
		public void Parse_NestedAlteredAssignment()
		{
			// Arrange
			const string code = "y = 5 + (x = 10)";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);

			Token yassign = result[0];
			Assert.That.TokenIsOperator(yassign, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(yassign[0], "y");

			Token add = yassign[1];
			Assert.That.TokenIsOperator(add, OperatorToken.Type.Additive, "+");
			Assert.That.TokenIsLiteralInteger(add[0], 5);
			Assert.That.TokenIsParentases(add[1], '(', 1);

			Token xassign = add[1][0];
			Assert.That.TokenIsOperator(xassign, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(xassign[0], "x");
			Assert.That.TokenIsLiteralInteger(xassign[1], 10);
		}

		[TestMethod]
		public void Parse_NestedHybridAssignment()
		{
			// Arrange
			const string code = "y = x += 10";

			// Act
			Token[] result = Parser.Parse(Tokenizer.Tokenize(code));

			// Assert
			CollectionAssert.That.TokensAreParsed(result);

			Token yassign = result[0];
			Assert.That.TokenIsOperator(yassign, OperatorToken.Type.Assignment, "=");
			Assert.That.TokenIsOfType<IdentifierToken>(yassign[0], "y");

			Token xassign = yassign[1];
			Assert.That.TokenIsOperator(xassign, OperatorToken.Type.Assignment, "+=");
			Assert.That.TokenIsOfType<IdentifierToken>(xassign[0], "x");

			Token add = xassign[1];
			Assert.That.TokenIsOperator(add, OperatorToken.Type.Additive, "+");
			Assert.That.TokenIsOfType<IdentifierToken>(add[0], "x");
			Assert.That.TokenIsLiteralInteger(add[1], 10);
		}
	}
}