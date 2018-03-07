using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RobotPlusPlus.Tests
{
	[TestClass]
	public class ParserExpressionSimpleTests
	{
		[TestMethod]
		public void Parse_Integer()
		{
			// Act
			string output = Parser.Parse("x=1");

			// Assert
			Assert.AreEqual("♥x=(int)1", output);
		}

		[TestMethod]
		public void Parse_IntegerWithSpaces()
		{
			// Act
			string output = Parser.Parse("x = 1");

			// Assert
			Assert.AreEqual("♥x=(int)1", output);
		}

		[TestMethod]
		public void Parse_IntegerWithLottaSpaces()
		{
			// Act
			string output = Parser.Parse("	x   =		  1   ");

			// Assert
			Assert.AreEqual("♥x=(int)1", output);
		}

		[TestMethod]
		public void Parse_DecimalPoint()
		{
			// Act
			string pointzero = Parser.Parse("x = 1.0");
			string pointnull = Parser.Parse("x = 1.");

			// Assert
			Assert.AreEqual("♥x=(float)1", pointzero);
			Assert.AreEqual("♥x=(float)1", pointnull);
		}

		[TestMethod]
		public void Parse_DecimalSuffix()
		{
			// Act
			string suffixupper = Parser.Parse("x = 1F");
			string suffixlower = Parser.Parse("x = 1f");

			// Assert
			Assert.AreEqual("♥x=(float)1", suffixupper);
			Assert.AreEqual("♥x=(float)1", suffixlower);
		}

		[TestMethod]
		public void Parse_DecimalPointAndSuffix()
		{
			// Act
			string pointzero_suffixupper = Parser.Parse("x = 1.0F");
			string pointzero_suffixlower = Parser.Parse("x = 1.0f");
			string pointnull_suffixupper = Parser.Parse("x = 1.F");
			string pointnull_suffixlower = Parser.Parse("x = 1.f");

			// Assert
			Assert.AreEqual("♥x=(float)1", pointzero_suffixupper);
			Assert.AreEqual("♥x=(float)1", pointzero_suffixlower);
			Assert.AreEqual("♥x=(float)1", pointnull_suffixupper);
			Assert.AreEqual("♥x=(float)1", pointnull_suffixlower);
		}

		[TestMethod]
		public void Parse_String()
		{
			// Act
			string output = Parser.Parse(@"x = ""foo""");

			// Assert
			Assert.AreEqual("♥x=‴foo‴", output);
		}

		[TestMethod]
		public void Parse_Boolean()
		{
			// Act
			string output = Parser.Parse("x = true");

			// Assert
			Assert.AreEqual("♥x=true", output);
		}

		[TestMethod]
		public void Parse_Variable()
		{
			// Act
			string output = Parser.Parse("x = y");

			// Assert
			Assert.AreEqual("♥x=♥y", output);
		}

	}
}