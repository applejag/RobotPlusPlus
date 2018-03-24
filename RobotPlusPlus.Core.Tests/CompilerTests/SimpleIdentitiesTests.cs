using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.Core.Tests.CompilerTests
{
	[TestClass]
	public class SimpleIdentitiesTests
	{
		[TestMethod]
		public void Compile_VariableCaseConflict()
		{
			const string code = "a=1 A=2";

			string output = Compiler.Compile(code);

			Assert.AreEqual("♥a=1\n♥a2=2", output);
		}
	}
}