using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Parsing;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;
using RobotPlusPlus.Tokenizing.Tokens.Literals;

namespace RobotPlusPlus.Tests.ParserTests
{
	[TestClass]
	public class SimpleAssignmentTests
	{
		[TestMethod]
		public void Parse_SimpleAssignment()
		{
			// Arrange
			const string input = "x = 4 * 3";

			// Act
			Token[] tokenized = Tokenizer.Tokenize(input);
			Token[] parsed = Parser.Parse(tokenized);

			// Assert
			Assert.IsNotNull(parsed);

			Token assi = parsed[0];
			Assert.That.TokenIsOperator(assi, Operator.Type.Assignment, "=");
			Assert.That.TokenIsOfType<Identifier>(assi[0], "x");

			Token mult = assi[1];
			Assert.That.TokenIsOperator(mult, Operator.Type.Multiplicative, "*");
			Assert.That.TokenIsLiteralInteger(mult[0], 4);
			Assert.That.TokenIsLiteralInteger(mult[1], 3);
		}

		[TestMethod]
		public void Parse_SimplePriorityCheck()
		{
			// Arrange
			const string input = "60 - 15 / (2 + 5)";

			// Act
			Token[] tokenized = Tokenizer.Tokenize(input);
			Token[] parsed = Parser.Parse(tokenized);

			// Assert
			Assert.IsNotNull(parsed);
			
			Token minus = parsed[0];
			Assert.That.TokenIsOperator(minus, Operator.Type.Additive, "-");
			Assert.That.TokenIsLiteralInteger(minus[0], 60);

			Token div = minus[1];
			Assert.That.TokenIsOperator(div, Operator.Type.Multiplicative, "/");
			Assert.That.TokenIsLiteralInteger(div[0], 15);

			Token par = div[1];
			Assert.That.TokenIsParentases(par, '(', 1);

			Token plus = par.Tokens[0];
			Assert.That.TokenIsOperator(plus, Operator.Type.Additive, "+");
			Assert.That.TokenIsLiteralInteger(plus[0], 2);
			Assert.That.TokenIsLiteralInteger(plus[1], 5);
		}
	}
}