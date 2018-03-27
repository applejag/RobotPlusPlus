using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Structures.G1ANT;
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

		[TestMethod]
		public void G1ANTRepositoryLoading()
		{
			G1ANTRepository repo = G1ANTRepository.FromEmbeddedXML();

			Assert.IsNotNull(repo);
			Assert.AreNotEqual(0, repo.GlobalArguments.Count);
			Assert.AreNotEqual(0, repo.Commands.Count);
		}
	}
}