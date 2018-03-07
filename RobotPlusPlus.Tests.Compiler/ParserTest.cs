using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RobotPlusPlus.Tests
{
    [TestClass]
    public class ParserTest
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Parse_NullInput()
		{
			// Arrange
			const string input = null;

			// Act
			Parser.Parse(input);

			// Assert
			Assert.Fail("Should've thrown an error on parseing");
		}

		[TestMethod]
		public void Parse_EmptyInput()
		{
			// Arrange
			const string input = "";

			// Act
			string output = Parser.Parse(input);

			// Assert
			Assert.AreEqual(input, output);
		}
	}
}
