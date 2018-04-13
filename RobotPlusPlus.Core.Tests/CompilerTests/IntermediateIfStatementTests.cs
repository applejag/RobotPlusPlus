using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateIfStatementTests
	{
		[TestMethod]
		public void Compile_IfStatementWithPostfix()
		{
			// Arrange
			const string code = "x=0 if x++ == 0 { x-- }";
			const string expected = "♥x=0\n" +
			                        "♥tmp=♥x==0\n" +
			                        "♥x=♥x+1\n" +
			                        "jump label ➜ifend if ⊂!♥tmp⊃\n" +
			                        "♥x=♥x-1\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfStatementWithFunctionValue()
		{
			// Arrange
			const string code = "if dialog.ask('foo') == 'bar' { x = 0 }";
			const string expected = "dialog.ask message ‴foo‴ result ♥tmp\n" +
			                        "jump label ➜ifend if ⊂!(♥tmp==\"bar\")⊃\n" +
			                        "♥x=0\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfStatementVariableAssignedOutside()
		{
			// Arrange
			const string code = "x = 0; if true { x = 1 } x = 2";
			const string expected = "♥x=0\n" +
			                        "jump label ➜ifend if ⊂!true⊃\n" +
			                        "♥x=1\n" +
			                        "➜ifend\n" +
			                        "♥x=2";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfStatementVariableAssignedInside()
		{
			// Arrange
			const string code = "if true { x = 1 } x = 2";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
			                        "♥x=1\n" +
			                        "➜ifend\n" +
			                        "♥x2=2";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfStatementVariableEmbeddedUsedInside()
		{
			// Arrange
			const string code = "if (x = 0) > 1 { x = 1 }";
			const string expected = "♥x=0\n" +
			                        "jump label ➜ifend if ⊂!(♥x>1)⊃\n" +
			                        "♥x=1\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfStatementVariableEmbeddedAssignedOutside()
		{
			// Arrange
			const string code = "if (x = 0) > 1 { x = 1 } x = 2";
			const string expected = "♥x=0\n" +
			                        "jump label ➜ifend if ⊂!(♥x>1)⊃\n" +
			                        "♥x=1\n" +
			                        "➜ifend\n" +
			                        "♥x2=2";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileUnassignedVariableException))]
		public void Compile_IfStatementVariableEmbeddedUsedOutside()
		{
			// Arrange
			const string code = "if (x = 0) > 1 { x = 1 } x++";

			// Act
			Compiler.Compile(code);
		}
	}
}