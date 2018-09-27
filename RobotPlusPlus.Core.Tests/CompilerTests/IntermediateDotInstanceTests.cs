using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateDotInstanceTests
	{
		[TestMethod]
		[ExpectedException(typeof(CompileFunctionValueOfVoidException))]
		public void Compile_CallOnVoidMethod()
		{
			// Arrange
			const string code = "x = screen.Inflate(screen.Size)";

		    // Act
		    string result = Compiler.Compile(code);

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }

		[TestMethod]
		public void Compile_StandaloneCallOnVoidMethod()
		{
			// Arrange
			const string code = "screen.Inflate(screen.Size)";
			const string expected = "♥screen=⊂new Func<System.Drawing.Rectangle>(()=>{var _=♥screen;_.Inflate(♥screen.Size);return _;})()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_PropertyCallOnVoidMethod()
		{
			// Arrange
			const string code = "screen.Location.Offset(screen.Location)";
			const string expected = "♥screen=⊂new Func<System.Drawing.Rectangle>(()=>{var _=♥screen.Location;_.Offset(♥screen.Location);return _;})()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
		
		[TestMethod]
		public void Compile_PropertyAssignment()
		{
			// Arrange
			const string code = "screen.X = 1";
			const string expected = "♥screen=⊂new Func<System.Drawing.Rectangle>(()=>{var _=♥screen;_.X=1;return _;})()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}