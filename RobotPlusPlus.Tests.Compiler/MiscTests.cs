using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Tests
{
	[TestClass]
	public class MiscTests
	{
		[TestMethod]
		public void MatchingParentasesTests()
		{
			char[] input = {'(', '[', '{'};
			char[] expected = {')', ']', '}'};

			char[] actual = input.Select(Punctuator.GetMatchingParentases).ToArray();

			CollectionAssert.AreEqual(expected, actual);
		}
	}
}