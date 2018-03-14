using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tests.CompilerTests
{
	[TestClass]
	public class SimpleExpressionTests
	{
		[TestMethod]
		public void Parse_Integer()
		{
			// Act
			string output = Compiler.Compile("x=1");

			// Assert
			Assert.AreEqual("♥x=1", output);
		}

		[TestMethod]
		public void Parse_IntegerWithSpaces()
		{
			// Act
			string output = Compiler.Compile("x = 1");

			// Assert
			Assert.AreEqual("♥x=1", output);
		}

		[TestMethod]
		public void Parse_IntegerWithLottaSpaces()
		{
			// Act
			string output = Compiler.Compile("	x   =		  1   ");

			// Assert
			Assert.AreEqual("♥x=1", output);
		}

		[TestMethod]
		public void Parse_DecimalPoint()
		{
			// Act
			string pointzero = Compiler.Compile("x = 1.0");
			string pointnull = Compiler.Compile("x = 1.");
			string pointb4 = Compiler.Compile("x = .0");

			// Assert
			Assert.AreEqual("♥x=1.0", pointzero);
			Assert.AreEqual("♥x=1.0", pointnull);
			Assert.AreEqual("♥x=0.0", pointb4);
		}

		[TestMethod]
		public void Parse_DecimalSuffix()
		{
			// Act
			string suffixupper = Compiler.Compile("x = 1F");
			string suffixlower = Compiler.Compile("x = 1f");

			// Assert
			Assert.AreEqual("♥x=1.0", suffixupper);
			Assert.AreEqual("♥x=1.0", suffixlower);
		}

		[TestMethod]
		public void Parse_DecimalPointAndSuffix()
		{
			// Act
			string pointzero_suffixupper = Compiler.Compile("x = 1.0F");
			string pointzero_suffixlower = Compiler.Compile("x = 1.0f");
			string pointnull_suffixupper = Compiler.Compile("x = 1.F");
			string pointnull_suffixlower = Compiler.Compile("x = 1.f");

			// Assert
			Assert.AreEqual("♥x=1.0", pointzero_suffixupper);
			Assert.AreEqual("♥x=1.0", pointzero_suffixlower);
			Assert.AreEqual("♥x=1.0", pointnull_suffixupper);
			Assert.AreEqual("♥x=1.0", pointnull_suffixlower);
		}

		[TestMethod]
		public void Parse_String()
		{
			// Act
			string output = Compiler.Compile(@"x = ""foo""");

			// Assert
			Assert.AreEqual("♥x=‴foo‴", output);
		}

		[TestMethod]
		public void Parse_Boolean()
		{
			// Act
			string output = Compiler.Compile("x = true");

			// Assert
			Assert.AreEqual("♥x=true", output);
		}

		[TestMethod]
		public void Parse_Variable()
		{
			// Act
			string output = Compiler.Compile("x = y");

			// Assert
			Assert.AreEqual("♥x=♥y", output);
		}

	}
}