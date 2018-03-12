using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Parsing;

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
			Parser.Compile(input);

			// Assert
			Assert.Fail("Should've thrown an error on parseing");
		}

		[TestMethod]
		public void Parse_EmptyInput()
		{
			// Arrange
			const string input = "";

			// Act
			string output = Parser.Compile(input);

			// Assert
			Assert.AreEqual(input, output);
		}
	}
}
