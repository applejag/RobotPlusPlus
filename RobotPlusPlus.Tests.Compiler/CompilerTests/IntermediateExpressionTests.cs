using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Exceptions;

namespace RobotPlusPlus.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateExpressionTests
	{
		[TestMethod]
		public void Compile_MultipleAssignmentsSpace()
		{
			// Act
			string output = Compiler.Compile("a = 1 b = 2");

			// Assert
			Assert.AreEqual("♥a=1\n♥b=2", output);
		}

		[TestMethod]
		public void Compile_MultipleAssignmentsNewLine()
		{
			// Act
			string output = Compiler.Compile("a = 1\nb = 2");

			// Assert
			Assert.AreEqual("♥a=1\n♥b=2", output);
		}

		[TestMethod]
		public void Compile_MultipleAssignmentsSemicolon()
		{
			// Act
			string output = Compiler.Compile("a = 1;b = 2");

			// Assert
			Assert.AreEqual("♥a=1\n♥b=2", output);
		}

		[TestMethod]
		public void Compile_StringEscape()
		{
			// Act
			string output = Compiler.Compile(@"x = ""foo\nbar""");

			// Assert
			Assert.AreEqual(@"♥x=⊂""foo\nbar""⊃", output);
		}

		[TestMethod]
		public void Compile_StringAndNumber()
		{
			// Act
			string output = Compiler.Compile(@"x = ""foo"" + 5");

			// Assert
			Assert.AreEqual(@"♥x=⊂""foo""+5⊃", output);
		}

		[TestMethod]
		[ExpectedException(typeof(UnassignedVariableException))]
		public void Compile_VariableUnassigned()
		{
			// Act
			Compiler.Compile("x = y");

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void Compile_VariableAssigned()
		{
			// Act
			string output = Compiler.Compile("y = 42     x = y");

			// Assert
			Assert.AreEqual("♥y=42\n♥x=♥y", output);
		}
	}
}