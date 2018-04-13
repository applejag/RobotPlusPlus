using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleWhileStatementTests
	{

		[TestMethod]
		public void Compile_WhileCodeBlockEmpty()
		{
			// Arrange
			const string code = "while true {}";
			const string expected = "➜while\n" +
									"jump label ➜while if ⊂true⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_WhileCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "while true { x = 1 }";
			const string expected = "jump label ➜whileend if ⊂!true⊃\n" +
									"➜while\n" +
										"♥x=1\n" +
									"jump label ➜while if ⊂true⊃\n" +
									"➜whileend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_WhileCodeBlockMultipleStatements()
		{
			// Arrange
			const string code = "while true { x = 1 z = x }";
			const string expected = "jump label ➜whileend if ⊂!true⊃\n" +
									"➜while\n" +
										"♥x=1\n" +
										"♥z=♥x\n" +
									"jump label ➜while if ⊂true⊃\n" +
									"➜whileend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_WhileSingleStatement()
		{
			// Arrange
			const string code = "while true x = 2";
			const string expected = "jump label ➜whileend if ⊂!true⊃\n" +
									"➜while\n" +
										"♥x=2\n" +
									"jump label ➜while if ⊂true⊃\n" +
									"➜whileend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_WhileOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "while true x = 2 z = 10";
			const string expected = "jump label ➜whileend if ⊂!true⊃\n" +
									"➜while\n" +
										"♥x=2\n" +
									"jump label ➜while if ⊂true⊃\n" +
									"➜whileend\n" +
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
			const string code = "while true { while false { a = 0 } }";
			const string expected = "jump label ➜whileend if ⊂!true⊃\n" +
									"➜while\n" +
										"jump label ➜whileend2 if ⊂!false⊃\n" +
										"➜while2\n" +
											"♥a=0\n" +
										"jump label ➜while2 if ⊂false⊃\n" +
										"➜whileend2\n" +
									"jump label ➜while if ⊂true⊃\n" +
									"➜whileend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}