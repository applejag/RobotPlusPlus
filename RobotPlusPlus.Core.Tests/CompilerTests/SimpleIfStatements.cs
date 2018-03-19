using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleIfStatements
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfNoCondition()
		{
			// Arrange
			const string code = "if {}";

			// Act
			Compiler.Compile(code);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void Compile_IfCodeBlockEmpty()
		{
			// Arrange
			const string code = "if true {}";
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
									"➜noif";

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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
			                        "♥x=1\n" +
			                        "➜noif";

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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
			                        "♥x=1\n" +
									"♥z=♥x\n" +
									"➜noif";

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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
			                        "♥x=2\n" +
			                        "➜noif";

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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
			                        "♥x=2\n" +
			                        "➜noif\n" +
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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
									"jump ➜noif2 if ⊂!(false)⊃\n" +
			                        "♥a=0\n" +
			                        "➜noif2\n" +
			                        "➜noif";

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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
									"jump ➜noif2 if ⊂!(false)⊃\n" +
			                        "♥a=0\n" +
			                        "➜noif2\n" +
			                        "➜noif";

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
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
									"jump ➜noif2 if ⊂!(false)⊃\n" +
			                        "♥a=0\n" +
			                        "➜noif2\n" +
			                        "➜noif";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}