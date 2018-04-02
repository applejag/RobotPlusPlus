using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateCommandTests
	{
		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_RequiredZeroArgs()
		{
			// Arrange
			const string code = "program()";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_RequiredNotAll()
		{
			// Arrange
			const string code = "test.equals(current: 5)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_RequiredWrongArg()
		{
			// Arrange
			const string code = "program(wait: true)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_RequiredValid()
		{
			// Arrange
			const string code = "program('notepad')";
			const string expected = "program name ‴notepad‴";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_RequiredGroupMissing()
		{
			// Arrange
			const string code = "excel.getvalue(row: 1)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_RequiredGroupValid_1()
		{
			// Arrange
			const string code = "excel.getvalue(row: 1, colindex: 1)";
			const string expected = "excel.getvalue row 1 colindex 1";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_RequiredGroupValid_2()
		{
			// Arrange
			const string code = "excel.getvalue(row: 1, colname: 'A')";
			const string expected = "excel.getvalue row 1 colname ‴A‴";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_RequiredGroupCollision()
		{
			// Arrange
			const string code = "excel.getvalue(row: 1, colindex: 1, colname: 'A')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_ReturnValue()
		{
			// Arrange
			const string code = "x = dialog.ask('hello world')";
			const string expected = "dialog.ask message ‴hello world‴ result ♥x";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_ReturnValueEmbedded()
		{
			// Arrange
			const string code = "y = 'lorem' + (x = dialog.ask('hello world'))";
			const string expected = "dialog.ask message ‴hello world‴ result ♥x\n" +
									"♥y=⊂\"lorem\"+♥x⊃";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_ReturnValueNoResult()
		{
			// Arrange
			const string code = "x = delay()";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_ReturnValueAltered()
		{
			// Arrange
			const string code = "x = 'lorem' + dialog.ask('hello world')";
			const string expected = "dialog.ask message ‴hello world‴ result ♥tmp\n" +
			                        "♥x=⊂\"lorem\"+♥tmp⊃";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_ResultLiteral()
		{
			// Arrange
			const string code = "directory.filescount(path: 'c:/', result: 'foo')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_ResultExpression()
		{
			// Arrange
			const string code = "x = 'foo'; directory.filescount(path: 'c:/', result: x + 'bar')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_ResultVariable()
		{
			// Arrange
			const string code = "dialog.ask('hello world', result: x)";
			const string expected = "dialog.ask message ‴hello world‴ result ♥x";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_DuplicateNamedArguments()
		{
			// Arrange
			const string code = "dialog(message: 'foo', message: 'bar')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_DuplicatePositionalArguments()
		{
			// Arrange
			const string code = "dialog('foo', message: 'bar')";

			// Act
			Compiler.Compile(code);
		}
	}
}