using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleIfElseStatementTests
	{
		[TestMethod]
		public void Compile_IfElseCodeBlockEmpty()
		{
			// Arrange
			const string code = "if true {} else {}";
			const string expected = "jump label ➜ifend if ⊂!true⊃\n" +
									"➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseCodeBlockEmptyElseSingleStatement()
		{
			// Arrange
			const string code = "if true { } else { x = 1 }";
			const string expected = "jump label ➜ifend if ⊂true⊃\n" +
									"♥x=1\n" +
									"➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseCodeBlockSingleStatement()
		{
			// Arrange
			const string code = "if true { x = 1 } else { z = 2 }";
			const string expected = "jump label ➜ifelse if ⊂!(true)⊃\n" +
			                        "♥x=1\n" +
			                        "jump label ➜ifend" +
			                        "➜ifelse\n" +
			                        "♥z=2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseVariableRedeclaration()
		{
			// Arrange
			const string code = "if true { x = 1 } else { x = 2 }";
			const string expected = "jump label ➜ifelse if ⊂!(true)⊃\n" +
			                        "♥x=1\n" +
			                        "jump label ➜ifend" +
			                        "➜ifelse\n" +
			                        "♥x2=2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseVariableReuse()
		{
			// Arrange
			const string code = "x = 0 if true { x = 1 } else { x = 2 }";
			const string expected = "♥x=0\n" +
			                        "jump label ➜ifelse if ⊂!(true)⊃\n" +
			                        "♥x=1\n" +
			                        "jump label ➜ifend" +
			                        "➜ifelse\n" +
			                        "♥x=2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileUnassignedVariableException))]
		public void Compile_IfElseVariableUnassigned()
		{
			// Arrange
			const string code = "if true { x = 1 } else { y = x }";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_IfElseSingleStatement()
		{
			// Arrange
			const string code = "if true x = 1 else z = 2";
			const string expected = "jump label ➜ifelse if ⊂!(true)⊃\n" +
									"♥x=1\n" +
									"jump label ➜ifend\n" +
									"➜ifelse\n" +
									"♥z=2\n" +
									"➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseOneStatementInsideOneOutside()
		{
			// Arrange
			const string code = "if true x = 2 else y = 4 z = 10";
			const string expected = "jump label ➜ifelse if ⊂!(true)⊃\n" +
									"♥x=1\n" +
									"jump label ➜ifend\n" +
									"➜ifelse\n" +
									"♥y=4\n" +
									"➜ifend\n" +
									"♥z=10";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseNestedBlocksAfterEmptyBlock()
		{
			// Arrange
			const string code = "if true { } else { if false a = 0 else b = 2 }";
			const string expected = "jump label ➜ifend if ⊂true⊃\n" +
			                        "jump label ➜ifelse if ⊂!(false)⊃\n" +
			                        "♥a=0\n" +
			                        "jump label ➜ifend\n" +
			                        "➜ifelse\n" +
			                        "♥b=2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfElseNestedEmptyBlocksAfterEmptyBlock()
		{
			// Arrange
			const string code = "if true { } else { if false { } else b = 2 }";
			const string expected = "jump label ➜ifend if ⊂true⊃\n" +
			                        "jump label ➜ifend if ⊂false⊃\n" +
			                        "♥b=2\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}


		[TestMethod]
		public void Compile_IfElseNestedEmptyBlocksWithAdditionalStatementAfterEmptyBlock()
		{
			// Arrange
			const string code = "if true { } else { if false { } else { b = 2 } a = 3 }";
			const string expected = "jump label ➜ifend if ⊂true⊃\n" +
			                        "jump label ➜ifend2 if ⊂false⊃\n" +
			                        "♥b=2\n" +
									"➜ifend2\n" +
									"♥a=3\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}