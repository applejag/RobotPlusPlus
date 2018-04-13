using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleDoWhileStatementTests
	{

		[TestMethod]
		public void Compile_DoWhileCodeBlockEmpty()
		{
			// Arrange
			const string code = "do {} while true";
			const string expected = "➜dowhile\n" +
									"jump label ➜dowhile if ⊂true⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_DoWhileCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "do { x = 1 } while true";
			const string expected = "➜dowhile\n" +
										"♥x=1\n" +
									"jump label ➜dowhile if ⊂true⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_DoWhileCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "do { x = 1 z = x } while true";
			const string expected = "➜dowhile\n" +
										"♥x=1\n" +
										"♥z=♥x\n" +
									"jump label ➜dowhile if ⊂true⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_DoWhileSingleStatement()
		{
			// Arrange
			const string code = "do x = 2 while true";
			const string expected = "➜dowhile\n" +
										"♥x=2\n" +
									"jump label ➜dowhile if ⊂true⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_DoWhileOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "do x = 2 while true z = 10";
			const string expected = "➜dowhile\n" +
										"♥x=2\n" +
									"jump label ➜dowhile if ⊂true⊃\n" +
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
			const string code = "do { do { a = 0 } while false } while true";
			const string expected = "➜dowhile\n" +
										"➜dowhile2\n" +
											"♥a=0\n" +
										"jump label ➜dowhile2 if ⊂false⊃\n" +
									"jump label ➜dowhile if ⊂true⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}