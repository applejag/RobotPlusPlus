using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Linguist.Compiling;

namespace RobotPlusPlus.Tests.CompilerTests
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