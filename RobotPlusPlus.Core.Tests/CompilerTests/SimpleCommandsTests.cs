using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleCommandsTests
	{
		[TestMethod]
		[ExpectedException(typeof(CompileUnexpectedTokenException))]
		public void Compile_Family_NoParentases()
		{
			// Arrange
			const string code = "delay";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_0Args()
		{
			// Arrange
			const string code = "dialog()";
			const string expected = "dialog";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Unnamed_Literal()
		{
			// Arrange
			const string code = "dialog('foo bar')";
			const string expected = "dialog message ‴foo bar‴";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Unamed_ExpressionStrings()
		{
			// Arrange
			const string code = "chrome('c:/some' + '/file')";
			const string expected = @"chrome url ⊂""c:/some""+""/file""⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Unamed_ExpressionNumbers()
		{
			// Arrange
			const string code = "delay(5 + 3)";
			const string expected = @"delay seconds 5+3";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Unnamed_VariableEmbedded()
		{
			// Arrange
			const string code = "dialog(x = 'lorem')";
			const string expected = "♥x=‴lorem‴\n" +
			                        "dialog message ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Unnamed_VariablePredefined()
		{
			// Arrange
			const string code = "x = 'lorem' dialog(x)";
			const string expected = "♥x=‴lorem‴\n" +
			                        "dialog message ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Named_Literal()
		{
			// Arrange
			const string code = "dialog(message: 'foo bar')";
			const string expected = "dialog message ‴foo bar‴";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Named_ExpressionStrings()
		{
			// Arrange
			const string code = "chrome(url: 'c:/some' + '/file')";
			const string expected = @"chrome url ⊂""c:/some""+""/file""⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Named_ExpressionNumbers()
		{
			// Arrange
			const string code = "delay(seconds: 5 + 3)";
			const string expected = @"delay seconds 5+3";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Named_VariableEmbedded()
		{
			// Arrange
			const string code = "dialog(message: x = 'lorem')";
			const string expected = "♥x=‴lorem‴\n" +
			                        "dialog message ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Named_VariablePredefined()
		{
			// Arrange
			const string code = "x = 'lorem' dialog(message: x)";
			const string expected = "♥x=‴lorem‴\n" +
			                        "dialog message ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_WrongArgName()
		{
			// Arrange
			const string code = "dialog(text: 'foo bar')";

			// Act
			Compiler.Compile(code);
		}


		[TestMethod]
		[ExpectedException(typeof(CompileFunctionException))]
		public void Compile_WrongCmdName()
		{
			// Arrange
			const string code = "opendialogbox(text: 'foo bar')";

			// Act
			Compiler.Compile(code);
		}
	}
}