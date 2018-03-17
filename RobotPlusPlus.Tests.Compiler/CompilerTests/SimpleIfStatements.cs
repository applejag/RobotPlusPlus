using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Exceptions;

namespace RobotPlusPlus.Tests.CompilerTests
{
	[TestClass]
	public class SimpleIfStatements
	{
		[TestMethod]
		[ExpectedException(typeof(ParseUnexpectedTrailingTokenException))]
		public void Compile_IfNoCondition()
		{
			// Arrange
			const string code = "if {}";

			// Act
			Compiler.Compile(code);

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void Compile_IfNoCode()
		{
			// Arrange
			const string code = "if true {}";
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
									"➜noif";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfWithStatement()
		{
			// Arrange
			const string code = "if true { x = 1 }";
			const string expected = "jump ➜noif if ⊂!(true)⊃\n" +
									"♥x=1\n" +
									"➜noif";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}