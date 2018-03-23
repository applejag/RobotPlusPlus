using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tests
{
	[TestClass]
	public class MiscTests
	{
		[TestMethod]
		public void MatchingParentasesTests()
		{
			char[] input = { '(', '[', '{' };
			char[] expected = { ')', ']', '}' };

			char[] actual = input.Select(PunctuatorToken.GetMatchingParentases).ToArray();

			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void StringEscaping()
		{
			const string input = "'\"\"\n\\\t";
			const string expected = @"'\""\""\n\\\t";

			string actual = input.EscapeString();

			Assert.AreEqual(expected, actual);
		}
	}
}