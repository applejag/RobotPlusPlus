using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class IntermediateIfStatementTests
	{
		[TestMethod]
		public void Compile_IfStatementWithPostfix()
		{
			// Arrange
			const string code = "x=0 if x++ == 0 { x-- }";
			const string expected = "♥x=0\n" +
									"♥tmp=♥x==0\n" +
									"♥x=♥x+1\n" +
			                        "jump label ➜ifend if ⊂!♥tmp⊃\n" +
									"♥x=♥x-1" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}