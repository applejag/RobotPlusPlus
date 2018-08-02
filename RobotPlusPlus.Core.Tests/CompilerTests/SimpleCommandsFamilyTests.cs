using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleCommandsFamilyTests
	{
		[TestMethod]
		[ExpectedException(typeof(CompileUnexpectedTokenException))]
		public void Compile_Family_NoParentases()
		{
			// Arrange
			const string code = "excel.open";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_Family_0Args()
		{
			// Arrange
			const string code = "excel.open()";
			const string expected = "excel.open";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Unnamed_Literal()
		{
			// Arrange
			const string code = "excel.open('c:/some/file')";
			const string expected = "excel.open path ‴c:/some/file‴";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Unnamed_VariableEmbedded()
		{
			// Arrange
			const string code = "excel.open(x = 'c:/some/file')";
			const string expected = "♥x=‴c:/some/file‴\n" +
			                        "excel.open path ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Unnamed_VariablePredefined()
		{
			// Arrange
			const string code = "x = 'c:/some/file' excel.open(x)";
			const string expected = "♥x=‴c:/some/file‴\n" +
			                        "excel.open path ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Named_Literal()
		{
			// Arrange
			const string code = "excel.open(path: 'c:/some/file')";
			const string expected = "excel.open path ‴c:/some/file‴";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Named_ExpressionStrings()
		{
			// Arrange
			const string code = "excel.open(path: 'c:/some' + '/file')";
			const string expected = @"excel.open path ⊂""c:/some""+""/file""⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Named_ExpressionNumbers()
		{
			// Arrange
			const string code = "excel.switch(id: 5 + 3)";
			const string expected = @"excel.switch id 5+3";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Named_VariableEmbedded()
		{
			// Arrange
			const string code = "excel.open(path: x = 'c:/some/file')";
			const string expected = "♥x=‴c:/some/file‴\n" +
			                        "excel.open path ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_Family_Named_VariablePredefined()
		{
			// Arrange
			const string code = "x = 'c:/some/file' excel.open(path: x)";
			const string expected = "♥x=‴c:/some/file‴\n" +
			                        "excel.open path ♥x";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileParameterNamedDoesntExistException))]
		public void Compile_Family_WrongArgName()
		{
			// Arrange
			const string code = "excel.open(text: 'c:/some/file')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_Family_WrongFamilyName()
		{
			// Arrange
			const string code = "computer.open(something: 'c:/some/file')";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypePropertyDoesNotExistException))]
		public void Compile_Family_WrongCmdName()
		{
			// Arrange
			const string code = "dialog.prompt(something: 'c:/some/file')";

			// Act
			Compiler.Compile(code);
		}
	}
}