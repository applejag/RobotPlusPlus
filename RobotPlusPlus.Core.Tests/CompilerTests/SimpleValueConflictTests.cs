using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleValueConflictTests
	{
		[TestMethod]
		public void Compile_OperatorSameType()
		{
			// Arrange
			const string code = "x = 20 - 5";
			const string expected = "♥x=20-5";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_OperatorNoConflict()
		{
			// Arrange
			const string code = "x = 'foo' + 5";
			const string expected = "♥x=⊂\"foo\"+5⊃";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeInvalidOperationException))]
		public void Compile_OperatorConflict()
		{
			// Arrange
			const string code = "x = 'foo' - 5";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitAssignmentException))]
		public void Compile_VariableTypeConflict()
		{
			// Arrange
			const string code = "x = 10; x = 'foo'";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_VariableReuseTypeNoConflict()
		{
			// Arrange
			const string code = "x = 'foo'; x += 10";
			const string expected = "♥x=‴foo‴\n" +
			                        "♥x=⊂♥x+10⊃";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitAssignmentException))]
		public void Compile_VariableReuseTypeConflict()
		{
			// Arrange
			const string code = "x = 20; x += 'boo'";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeInvalidOperationException))]
		public void Compile_VariableReuseInvalidOperation()
		{
			// Arrange
			const string code = "x = 20; x -= 'boo'";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_CommandResultCorrectType()
		{
			// Arrange
			const string code = "x = 'foo'; x = dialog.ask('lorem')";
			const string expected = "♥x=‴foo‴\n" +
									"dialog.ask message ‴lorem‴ result ♥x";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitAssignmentException))]
		public void Compile_CommandResultIncorrectType()
		{
			// Arrange
			const string code = "x = 90; x = dialog.ask('lorem')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_CommandArgumentCorrectType()
		{
			// Arrange
			const string code = "dialog('lorem')";
			const string expected = "dialog message ‴lorem‴";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitCommandArgumentException))]
		public void Compile_CommandArgumentIncorrectType()
		{
			// Arrange
			const string code = "dialog(true)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_IfNonBoolVariable()
		{
			// Arrange
			const string code = "x = 10; if x {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_IfNonBoolLiteral()
		{
			// Arrange
			const string code = "if 10 {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_WhileNonBoolVariable()
		{
			// Arrange
			const string code = "x = 10; while x {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_WhileNonBoolLiteral()
		{
			// Arrange
			const string code = "while 10 {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_DoWhileNonBoolVariable()
		{
			// Arrange
			const string code = "x = 10; do {} while x";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_DoWhileNonBoolLiteral()
		{
			// Arrange
			const string code = "do {} while 10";

			// Act
			Compiler.Compile(code);
		}

	}
}