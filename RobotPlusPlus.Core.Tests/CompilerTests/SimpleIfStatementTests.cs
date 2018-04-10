using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleIfStatementTests
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfNoCondition()
		{
			// Arrange
			const string code = "if {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfNoBlock()
		{
			// Arrange
			const string code = "if 1 > 0";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_IfCodeBlockEmpty()
		{
			// Arrange
			const string code = "if true {}";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
									"➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "if true { x = 1 }";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
			                        "♥x=1\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "if true { x = 1 z = x }";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
			                        "♥x=1\n" +
									"♥z=♥x\n" +
									"➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfSingleStatement()
		{
			// Arrange
			const string code = "if true x = 2";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
			                        "♥x=2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "if true x = 2 z = 10";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
			                        "♥x=2\n" +
			                        "➜ifend\n" +
			                        "♥z=10";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfNestedBlocks()
		{
			// Arrange
			const string code = "if true { if false { a = 0 } }";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
									"jump label ➜ifend2 if ⊂!false⊃\n" +
			                        "♥a=0\n" +
			                        "➜ifend2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfNestedFirstHasBlock()
		{
			// Arrange
			const string code = "if true { if false a = 0 }";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
									"jump label ➜ifend2 if ⊂!false⊃\n" +
			                        "♥a=0\n" +
			                        "➜ifend2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfNestedSecondHasBlock()
		{
			// Arrange
			const string code = "if true if false { a = 0 }";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
			                        "jump label ➜ifend2 if ⊂!false⊃\n" +
			                        "♥a=0\n" +
			                        "➜ifend2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}


		[TestMethod]
		public void Compile_IfEmbeddedAssignment()
		{
			// Arrange
			const string code = "if (x = 2) > 1 { }";
			const string expected = "♥x=2\n" +
			                        "jump label ➜ifend if ⊂!(♥x>1)⊃\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}