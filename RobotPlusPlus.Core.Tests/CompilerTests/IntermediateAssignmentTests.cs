using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateAssignmentTests
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
		[ExpectedException(typeof(UnassignedVariableException))]
		public void Compile_VariableSelfAssign()
		{
			// Act
			Compiler.Compile("x = x");

			// Assert
			Assert.Fail();
		}


		[TestMethod]
		[ExpectedException(typeof(UnassignedVariableException))]
		public void Compile_VariableSelfIncrementAssign()
		{
			// Act
			Compiler.Compile("x += 10");

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

		[TestMethod]
		public void Compile_NestedAssignment()
		{
			// Act
			string output = Compiler.Compile("y = x = 10");

			// Assert
			Assert.AreEqual("♥x=10\n♥y=♥x", output);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedLeadingTokenException))]
		public void Compile_NestedInvalidAssignment()
		{
			// Act
			// Should compile to y = ((5 + x) = 10), which is invalid
			// You can't set (5 + x) = 10, need solo variable
			Compiler.Compile("y = 5 + x = 10");

			// Assert
		}

		[TestMethod]
		public void Compile_NestedAlteredAssignment()
		{
			// Act
			string output = Compiler.Compile("y = 5 + (x = 10)");

			// Assert
			Assert.AreEqual("♥x=10\n♥y=5+♥x", output);
		}
		
		[TestMethod]
		public void Compile_NestedMultipleAssignment()
		{
			// Act
			string output = Compiler.Compile("y = x = z = 10");

			// Assert
			Assert.AreEqual("♥z=10\n♥x=♥z\n♥y=♥x", output);
		}

	}
}