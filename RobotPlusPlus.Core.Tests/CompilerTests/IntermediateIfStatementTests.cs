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
			                        "♥x=♥x-1\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}

		[TestMethod]
		public void Compile_IfStatementWithFunctionValue()
		{
			// Arrange
			const string code = "if dialog.ask('foo') == 'bar' { x = 0 }";
			const string expected = "dialog.ask message ‴foo‴ result ♥tmp\n" +
			                        "jump label ➜ifend if ⊂!(♥tmp==\"bar\")⊃\n" +
			                        "♥x=0\n" +
			                        "➜ifend";

			// Act
			string compiled = Compiler.Compile(code);

			// Assert
			Assert.AreEqual(expected, compiled);
		}
	}
}