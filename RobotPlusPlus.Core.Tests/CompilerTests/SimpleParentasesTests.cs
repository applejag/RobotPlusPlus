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
			Assert.That.AreCodeEqual("♥x=1", output);
		}

		[TestMethod]
		public void Compile_DoubleParentasesAroundLiteral()
		{
			// Arrange
			string output = Compiler.Compile("x=((1))");

			// Assert
			Assert.That.AreCodeEqual("♥x=1", output);
		}

		[TestMethod]
		public void Compile_ParentasesInIfStatement()
		{
			// Arrange
			string output = Compiler.Compile("if (1 > 0) {}");
			const string expected = "jump label ➜ifend if ⊂1>0⊃\n" +
			                        "➜ifend";

			// Assert
			Assert.That.AreCodeEqual(expected, output);
		}

		[TestMethod]
		public void Compile_DoubleParentasesInIfStatement()
		{
			// Arrange
			string output = Compiler.Compile("if ((1 > 0)) {}");
			const string expected = "jump label ➜ifend if ⊂1>0⊃\n" +
			                        "➜ifend";

			// Assert
			Assert.That.AreCodeEqual(expected, output);
		}

		[TestMethod]
		public void Compile_ParentasesPriorityNotNeeded()
		{
			// Act
			string output = Compiler.Compile("x=(1*5)+5");

			// Assert
			Assert.That.AreCodeEqual("♥x=1*5+5", output);
		}

		[TestMethod]
		public void Compile_ParentasesPriorityNeeded()
		{
			// Act
			string output = Compiler.Compile("x=1*(5+5)");

			// Assert
			Assert.That.AreCodeEqual("♥x=1*(5+5)", output);
		}
	}
}