using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleBuiltInVariableTests
	{
		[TestMethod]
		public void Compile_VariableUsePredefined()
		{
			// Arrange
			const string code = "x = clipboard";
			const string expected = "♥x=♥clipboard";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_VariableUseUndefined()
		{
			// Arrange
			const string code = "x = tripboard";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_VariableWriteString()
		{
			// Arrange
			const string code = "clipboard = 'hello'";
			const string expected = "♥clipboard=‴hello‴";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_VariableWriteInteger()
		{
			// Arrange
			const string code = "timeout = 5000";
			const string expected = "♥timeout=5000";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_VariableWriteBoolean()
		{
			// Arrange
			const string code = "capslock = true";
			const string expected = "♥capslock=true";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitAssignmentException))]
		public void Compile_VariableWriteWrongType()
		{
			// Arrange
			const string code = "clipboard = 205";

			// Act
			Compiler.Compile(code);
		}
	}
}