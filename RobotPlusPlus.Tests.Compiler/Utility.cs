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

		public static void AssertTokenTypesAllSame(IReadOnlyList<Token> tokens, TokenType type)
		{
			Assert.IsNotNull(tokens, "Tokens list is null.");

			for (var i = 0; i < tokens.Count; i++)
			{
				Assert.IsNotNull(tokens[i], $"tokens[{i}] is null.");
				Assert.AreEqual(type, tokens[i].Type, $"tokens[{i}] wrong type.");
			}
		}

		public static void ActAndAssert(IEnumerable<string> samples, params TokenType[] expected)
		{
			foreach (string input in samples)
			{
				try
				{
					// Act
					Token[] result = Tokenizer.Tokenize(input);

					// Assert
					AssertTokenTypes(result, expected);
				}
				catch (ParseException e)
				{
					throw new AssertFailedException($"Failed on:<{input}>. {e.Message}", e);
				}
			}
		}

	}
}