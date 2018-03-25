using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleIdentitiesTests
	{
		[TestMethod]
		public void Compile_VariableCaseConflict()
		{
			const string code = "a=1 A=2";

			string output = Compiler.Compile(code);

			Assert.AreEqual("♥a=1\n♥A2=2", output);
		}

		[TestMethod]
		public void Compile_VariableReuseInBlock()
		{
			// Act
			string output = Compiler.Compile("x = 10 { x = 15 }");

			// Assert
			Assert.AreEqual("♥x=10\n♥x=15", output);
		}

		[TestMethod]
		public void Compile_VariableOutsideBlock()
		{
			// Act
			string output = Compiler.Compile("{ x = 10 } x = 15");

			// Assert
			Assert.AreEqual("♥x=10\n♥x2=15", output);
		}

		[TestMethod]
		public void Compile_VariableSeperateBlocks()
		{
			// Act
			string output = Compiler.Compile("{ x = 10 } { x = 15 }");

			// Assert
			Assert.AreEqual("♥x=10\n♥x2=15", output);
		}

		[TestMethod]
		public void Compile_VariableOutsideNestedBlock()
		{
			// Act
			string output = Compiler.Compile("{ { x = 10 y = 2 } x = 15 } y = 8");

			// Assert
			Assert.AreEqual("♥x=10\n♥y=2\n♥x2=15\n♥y2=8", output);
		}
	}
}