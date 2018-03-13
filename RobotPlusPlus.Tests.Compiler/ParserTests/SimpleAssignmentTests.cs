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

			// assignment
			var assignment = parsed[0] as Operator;
			Assert.IsNotNull(assignment);
			Assert.AreEqual(assignment.OperatorType, Operator.Type.Assignment);
			Assert.AreEqual(assignment.SourceCode, "=");

			// LHS: identifier x
			var assignmentLHS = assignment.LHS as Identifier;
			Assert.IsNotNull(assignmentLHS);
			Assert.AreEqual(assignmentLHS.SourceCode, "x");

			// RHS: operator *
			var assignmentRHS = assignment.RHS as Operator;
			Assert.IsNotNull(assignmentRHS);
			Assert.AreEqual(assignmentRHS.OperatorType, Operator.Type.Multiplicative);
			Assert.AreEqual(assignmentRHS.SourceCode, "*");

			// RHS: operator *, LHS: literal 4
			var mult4 = assignmentRHS.LHS as LiteralNumber;
			Assert.IsNotNull(mult4);
			Assert.IsTrue(mult4.IsInteger);
			Assert.IsFalse(mult4.IsReal);
			Assert.AreEqual(mult4.Value, 4);

			// RHS: operator *, RHS: literal 3
			var mult3 = assignmentRHS.RHS as LiteralNumber;
			Assert.IsNotNull(mult3);
			Assert.IsTrue(mult3.IsInteger);
			Assert.IsFalse(mult3.IsReal);
			Assert.AreEqual(mult3.Value, 3);
		}

		[TestMethod]
		public void Parse_SimplePriorityCheck()
		{
			// Arrange
			const string input = "60 - 15 / (2 - 5)";

			// Act
			Token[] tokenized = Tokenizer.Tokenize(input);
			Token[] parsed = Parser.Parse(tokenized);

			// Assert
			Assert.IsNotNull(parsed);

			// assignment
			var assignment = parsed[0] as Operator;
			Assert.IsNotNull(assignment);
			Assert.AreEqual(assignment.SourceCode, "=");

			// LHS: identifier x
			var assignmentLHS = assignment.LHS as Identifier;
			Assert.IsNotNull(assignmentLHS);
			Assert.AreEqual(assignmentLHS.SourceCode, "x");

			// RHS: operator *
			var assignmentRHS = assignment.RHS as Operator;
			Assert.IsNotNull(assignmentRHS);
			Assert.AreEqual(assignmentRHS.OperatorType, Operator.Type.Multiplicative);
			Assert.AreEqual(assignmentRHS.SourceCode, "*");

			// RHS: operator *, LHS: literal 4
			var mult4 = assignmentRHS.LHS as LiteralNumber;
			Assert.IsNotNull(mult4);
			Assert.IsTrue(mult4.IsInteger);
			Assert.IsFalse(mult4.IsReal);
			Assert.AreEqual(mult4.Value, 4);

			// RHS: operator *, RHS: literal 3
			var mult3 = assignmentRHS.RHS as LiteralNumber;
			Assert.IsNotNull(mult3);
			Assert.IsTrue(mult3.IsInteger);
			Assert.IsFalse(mult3.IsReal);
			Assert.AreEqual(mult3.Value, 3);
		}
	}
}