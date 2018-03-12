using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Tests
{
	public static class Utility
	{
		public static void AssertTokenTypes(IReadOnlyList<Token> tokens, params Type[] types)
		{
			Assert.IsNotNull(tokens, "Tokens list is null.");
			Assert.AreEqual(types.Length, tokens.Count, "Wrong token count in result.");

			CollectionAssert.AllItemsAreNotNull(tokens.ToList());
			
			for (var i = 0; i < types.Length; i++)
			{
				Assert.IsInstanceOfType(tokens[i], types[i], $"tokens[{i}] wrong type.");
			}
		}

		public static void AssertTokenTypesAllSame(Token[] tokens, Type type)
		{
			Assert.IsNotNull(tokens, "Tokens list is null.");
			CollectionAssert.AllItemsAreNotNull(tokens);
			
			for (var i = 0; i < tokens.Length; i++)
			{
				Assert.IsInstanceOfType(tokens[i], type, $"tokens[{i}] wrong type.");
			}
		}

		public static void ActAndAssert(IEnumerable<string> samples, params Type[] expected)
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