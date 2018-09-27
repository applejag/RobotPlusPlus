using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateDotStaticTests
	{

		[TestMethod]
		[ExpectedException(typeof(CompileFunctionValueOfVoidException))]
		public void Compile_CallOnVoidMethod()
		{
			// Arrange
			const string code = "x = Console.Write('hello world')";
            
		    // Act
		    string result = Compiler.Compile(code);

		    // Assert
		    Assert.Fail("Unexpected result: {0}", result);
        }

		[TestMethod]
		public void Compile_StandaloneCallOnVoidMethod()
		{
			// Arrange
			const string code = "Console.Write('hello world')";
			const string expected = "♥_=⊂new Func<int>(()=>{Console.Write(\"hello world\");return 0;})()⊃";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}