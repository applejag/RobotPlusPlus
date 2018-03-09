using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Tests
{
	public static class Utility
	{
		public static void AssertTokenTypes(IReadOnlyList<Token> tokens, params TokenType[] types)
		{
			Assert.IsNotNull(tokens, "Tokens list is null.");
			Assert.AreEqual(types.Length, tokens.Count, "Wrong token count in result.");

			for (var i = 0; i < tokens.Count; i++)
			{
				Assert.IsNotNull(tokens[i], $"tokens[{i}] is null.");
				Assert.AreEqual(types[i], tokens[i].Type, $"tokens[{i}] wrong type.");
			}
		}
	}
}