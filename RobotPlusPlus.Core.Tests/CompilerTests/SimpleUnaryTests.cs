using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleUnaryTests
	{
		[TestMethod]
		public void Compile_NumberNegative()
		{
			// Arrange
			const string code = "x = -50";
			const string expected = "♥x=-50";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_NumberNegativeNegative()
		{
			// Arrange
			const string code = "x = - -50";
			const string expected = "♥x=50";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_NumberPositive()
		{
			// Arrange
			const string code = "x = +50";
			const string expected = "♥x=50";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_ParentasesNegative()
		{
			// Arrange
			const string code = "x = -(50 + 5)";
			const string expected = "♥x=-(50+5)";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_CommandNegative()
		{
			// Arrange
			const string code = "x = -directory.filescount('c:/')";
			const string expected = "directory.filescount path ‴c:/‴ result ♥tmp\n" +
			                        "♥x=-♥tmp";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_NegateNegateBoolean()
		{
			// Arrange
			const string code = "x = !!true";
			const string expected = "♥x=true";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_PrefixUnassignedVariable()
		{
			// Arrange
			const string code = "++x";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		[ExpectedException(typeof(CompileVariableUnassignedException))]
		public void Compile_PostfixUnassignedVariable()
		{
			// Arrange
			const string code = "x++";

			// Act
			Compiler.Compile(code);
		}

		[TestMethod]
		public void Compile_PrefixVariable()
		{
			// Arrange
			const string code = "x = 0; ++x";
			const string expected = "♥x=0\n" +
			                        "♥x=♥x+1";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_PostfixVariable()
		{
			// Arrange
			const string code = "x = 0; x++";
			const string expected = "♥x=0\n" +
			                        "♥x=♥x+1";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_PrefixExpressionVariable()
		{
			// Arrange
			const string code = "x = 0; y = ++x";
			const string expected = "♥x=0\n" +
			                        "♥x=♥x+1\n" +
									"♥y=♥x";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_PostfixExpressionVariable()
		{
			// Arrange
			const string code = "x = 0; y = x++";
			const string expected = "♥x=0\n" +
			                        "♥y=♥x\n" +
			                        "♥x=♥x+1";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_PrefixNegativeExpressionVariable()
		{
			// Arrange
			const string code = "x = 0; y = -++x";
			const string expected = "♥x=0\n" +
			                        "♥x=♥x+1\n" +
			                        "♥y=-♥x";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Compile_PostfixNegativeExpressionVariable()
		{
			// Arrange
			const string code = "x = 0; y = -x++";
			const string expected = "♥x=0\n" +
			                        "♥y=-♥x\n" +
			                        "♥x=♥x+1";

			// Act
			string actual = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, actual);
		}
	}
}