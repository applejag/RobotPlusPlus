using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
    [TestClass]
    public class SimpleTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Parse_NullInput()
		{
			// Arrange
			const string input = null;

			// Act
			Compiler.Compile(input);

			// Assert
			Assert.Fail("Should've thrown an error on parseing");
		}

		[TestMethod]
		public void Parse_EmptyInput()
		{
			// Arrange
			const string input = "";

			// Act
			string output = Compiler.Compile(input);

			// Assert
			Assert.AreEqual(input, output);
		}
	}
}
