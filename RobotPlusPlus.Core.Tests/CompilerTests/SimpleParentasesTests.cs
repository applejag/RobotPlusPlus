using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleParentasesTests
	{

		[TestMethod]
		public void Compile_ParentasesAroundLiteral()
		{
			// Arrange
			string output = Compiler.Compile("x=(1)");

			// Assert
			Assert.AreEqual("♥x=1", output);
		}

		[TestMethod]
		public void Compile_DoubleParentasesAroundLiteral()
		{
			// Arrange
			string output = Compiler.Compile("x=((1))");

			// Assert
			Assert.AreEqual("♥x=1", output);
		}

		[TestMethod]
		public void Compile_ParentasesInIfStatement()
		{
			// Arrange
			string output = Compiler.Compile("if (x > 0)");
			const string expected = "jump ➜noif if ⊂!(x>0)⊃\n" +
			                        "➜noif";

			// Assert
			Assert.AreEqual(expected, output);
		}

		[TestMethod]
		public void Compile_DoubleParentasesInIfStatement()
		{
			// Arrange
			string output = Compiler.Compile("if ((x > 0))");
			const string expected = "jump ➜noif if ⊂!(x>0)⊃\n" +
			                        "➜noif";

			// Assert
			Assert.AreEqual(expected, output);
		}

		[TestMethod]
		public void Compile_ParentasesPriorityNotNeeded()
		{
			// Act
			string output = Compiler.Compile("x=(1*5)+5");

			// Assert
			Assert.AreEqual("♥x=1*5+5", output);
		}

		[TestMethod]
		public void Compile_ParentasesPriorityNeeded()
		{
			// Act
			string output = Compiler.Compile("x=1*(5+5)");

			// Assert
			Assert.AreEqual("♥x=1*(5+5)", output);
		}
	}
}