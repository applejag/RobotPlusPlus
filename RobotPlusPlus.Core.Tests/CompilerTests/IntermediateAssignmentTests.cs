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
            const string expected = "♥a=1\n♥b=2";

			// Act
			string actual = Compiler.Compile("a = 1 b = 2");

            // Assert
		    Assert.That.AreCodeEqual(expected, actual);
        }

		[TestMethod]
		public void Compile_MultipleAssignmentsNewLine()
		{
            const string expected = "♥a=1\n♥b=2";

			// Act
			string actual = Compiler.Compile("a = 1\nb = 2");

            // Assert
		    Assert.That.AreCodeEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_MultipleAssignmentsSemicolon()
		{
            const string expected = "♥a=1\n♥b=2";

			// Act
			string actual = Compiler.Compile("a = 1;b = 2");

            // Assert
		    Assert.That.AreCodeEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_StringEscape()
		{
            const string expected = @"♥x=⊂""foo\nbar""⊃";

			// Act
			string actual = Compiler.Compile(@"x = ""foo\nbar""");

            // Assert
		    Assert.That.AreCodeEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_StringAndNumber()
		{
            const string expected = @"♥x=⊂""foo""+5⊃";

			// Act
			string actual = Compiler.Compile(@"x = ""foo"" + 5");

			// Assert
		    Assert.That.AreCodeEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_VariableUnassigned()
		{
			// Act
			string result = Compiler.Compile("x = y");

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_VariableSelfAssign()
		{
			// Act
			string result = Compiler.Compile("x = x");

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }


		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_VariableSelfIncrementAssign()
		{
			// Act
			string result = Compiler.Compile("x += 10");

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }

		[TestMethod]
		public void Compile_VariableAssigned()
		{
			// Act
			string actual = Compiler.Compile("y = 42     x = y");

			// Assert
			Assert.That.AreCodeEqual("♥y=42\n♥x=♥y", actual);
		}

		[TestMethod]
		public void Compile_NestedAssignment()
		{
			// Act
			string actual = Compiler.Compile("y = x = 10");

			// Assert
			Assert.That.AreCodeEqual("♥x=10\n♥y=♥x", actual);
		}

		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedLeadingTokenException))]
		public void Compile_NestedInvalidAssignment()
		{
			// Act
			// Should compile to y = ((5 + x) = 10), which is invalid
			// You can't set (5 + x) = 10, need solo variable
		    string result = Compiler.Compile("y = 5 + x = 10");

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }

		[TestMethod]
		public void Compile_NestedAlteredAssignment()
		{
			// Act
			string actual = Compiler.Compile("y = 5 + (x = 10)");

			// Assert
			Assert.That.AreCodeEqual("♥x=10\n♥y=5+♥x", actual);
		}

		[TestMethod]
		public void Compile_NestedMultipleAssignment()
		{
			// Act
			string actual = Compiler.Compile("y = x = z = 10");

			// Assert
			Assert.That.AreCodeEqual("♥z=10\n♥x=♥z\n♥y=♥x", actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(CompileTypeReadOnlyException))]
		public void Compile_AssignStaticValue()
		{
		    // Act
		    string result = Compiler.Compile("string = 'foo'");

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }

	    [TestMethod]
	    public void Compile_AssignInCondition()
	    {
	        const string code = "if (x = true) {}";
	        const string expected = "♥x=true\n" +
	                                "jump label ➜ifend if ⊂♥x⊃\n" +
	                                "➜ifend";

	        // Act
	        string actual = Compiler.Compile(code);

	        // Assert
	        Assert.That.AreCodeEqual(expected, actual);
	    }

	    [TestMethod]
	    public void Compile_AssignInConditionWithComparison()
	    {
	        const string code = "if ((x = true) == true) {}";
	        const string expected = "♥x=true\n" +
	                                "jump label ➜ifend if ⊂♥x==true⊃\n" +
	                                "➜ifend";

	        // Act
	        string actual = Compiler.Compile(code);

	        // Assert
	        Assert.That.AreCodeEqual(expected, actual);
	    }

	    [TestMethod]
	    public void Compile_AssignInConditionWithComparisonAndMethod()
	    {
	        const string code = "if ((x = true).ToString() == 'true') {}";
	        const string expected = "♥x=true\n" +
	                                "jump label ➜ifend if ⊂♥x.ToString()==\"true\"⊃\n" +
	                                "➜ifend";

	        // Act
	        string actual = Compiler.Compile(code);

	        // Assert
	        Assert.That.AreCodeEqual(expected, actual);
	    }

    }
}