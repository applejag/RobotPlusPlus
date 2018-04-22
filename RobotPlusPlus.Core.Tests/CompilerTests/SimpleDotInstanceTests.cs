using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleDotInstanceTests
	{
		[TestMethod]
		public void Compile_Property()
		{
			// Arrange
			const string code = "x = screen.Width";
			const string expected = "♥x=⊂♥screen.Width⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_PropertyOfProperty()
		{
			// Arrange
			const string code = "x = screen.Size.Width";
			const string expected = "♥x=⊂♥screen.Size.Width⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypePropertyDoesNotExistException))]
		public void Compile_UndefinedProperty()
		{
			// Arrange
			const string code = "x = screen.Lorem";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_PropertyOfUndefined()
		{
			// Arrange
			const string code = "x = lorem.Width";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypePropertyDoesNotExistException))]
		public void Compile_PropertyStatic()
		{
			// Arrange
			const string code = "x = 'hello'.Empty";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypePropertyDoesNotExistException))]
		public void Compile_UndefinedPropertyOfProperty()
		{
			// Arrange
			const string code = "x = screen.Size.Lorem";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypePropertyDoesNotExistException))]
		public void Compile_PropertyOfUndefinedProperty()
		{
			// Arrange
			const string code = "x = screen.Lorem.Width";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_PropertyOfMethodResult()
		{
			// Arrange
			const string code = "x = screen.ToString().Length";
			const string expected = "♥x=⊂♥screen.ToString().Length⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_CallOnString_0Arg()
		{
			// Arrange
			const string code = "x = 'foo'.ToUpper()";
			const string expected = "♥x=⊂\"foo\".ToUpper()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_CallOnString_1Arg()
		{
			// Arrange
			const string code = "x = 'foo'.EndsWith('oo')";
			const string expected = "♥x=⊂\"foo\".EndsWith(\"oo\")⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Compile_CallOnString_WrongArgType()
		{
			// Arrange
			const string code = "x = 'foo'.PadLeft(true)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Compile_CallOnString_NonExisting()
		{
			// Arrange
			const string code = "x = 'foo'.LoremIpsum()";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_CallOnNumber_0Arg()
		{
			// Arrange
			const string code = "x = 10.ToString()";
			const string expected = "♥x=⊂10.ToString()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_CallOnNumber_1Arg()
		{
			// Arrange
			const string code = "x = 10.CompareTo(20)";
			const string expected = "♥x=⊂10.CompareTo(20)⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Compile_CallOnNumber_WrongArgType()
		{
			// Arrange
			const string code = "x = 10.CompareTo(true)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Compile_CallOnNumber_NonExisting()
		{
			// Arrange
			const string code = "x = 10.LoremIpsum()";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_CallOnVariable_0Arg()
		{
			// Arrange
			const string code = "x = 'foovar'; y = x.ToUpper()";
			const string expected = "♥x=‴foovar‴\n" +
									"♥y=⊂♥x.ToUpper()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_CallOnVariable_1Arg()
		{
			// Arrange
			const string code = "x = 'foovar'; y = x.EndsWith('var')";
			const string expected = "♥x=‴foovar‴\n" +
			                        "♥y=⊂♥x.EndsWith(\"var\")⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_CallOnVariable_2Arg()
		{
			// Arrange
			const string code = "x = screen.Contains(1, 2)";
			const string expected = "♥x=⊂♥screen.Contains(1,2)⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Compile_CallOnVariable_WrongArgType()
		{
			// Arrange
			const string code = "x = 'foovar'; y = x.PadLeft(true)";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Compile_CallOnVariable_NonExisting()
		{
			// Arrange
			const string code = "x = 'foovar'; y = x.LoremIpsum()";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_UseCallResultInIf()
		{
			// Arrange
			const string code = "if 'a'.Equals('b') {}";
			const string expected = "jump label ➜ifend if ⊂\"a\".Equals(\"b\")⊃\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_UseCallResultInIf_WrongType()
		{
			// Arrange
			const string code = "if 'a'.ToUpper() {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_UsePropertyResultInIf()
		{
			// Arrange
			const string code = "if 'a'.Length == 0 {}";
			const string expected = "jump label ➜ifend if ⊂\"a\".Length==0⊃\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileTypeConvertImplicitException))]
		public void Compile_UsePropertyResultInIf_WrongType()
		{
			// Arrange
			const string code = "if 'a'.Length {}";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_CallOnProperty()
		{
			// Arrange
			const string code = "x = screen.Size.ToString()";
			const string expected = "♥x=⊂♥screen.Size.ToString()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}