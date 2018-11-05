using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tests
{
	public static class Utility
	{
		[AssertionMethod]
		public static void TokenIsOperator(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, OperatorToken.Type type)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(OperatorToken));
			Assert.AreEqual(type, ((OperatorToken)token).OperatorType);
		}

		[AssertionMethod]
		public static void TokenIsOperator(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, OperatorToken.Type type, string expectedSource)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(OperatorToken));
			Assert.AreEqual(type, ((OperatorToken)token).OperatorType);
			Assert.AreEqual(expectedSource, ((OperatorToken)token).SourceCode);
		}

		[AssertionMethod]
		public static void TokenIsLiteralInteger(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, int value)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(LiteralNumberToken));
			Assert.IsTrue(((LiteralNumberToken)token).IsInteger);
			Assert.AreEqual(value, ((LiteralNumberToken)token).Value);
		}

		[AssertionMethod]
		public static void TokenIsLiteralReal(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, double value)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(LiteralNumberToken));
			Assert.IsTrue(((LiteralNumberToken)token).IsReal);
			Assert.AreEqual(value, ((LiteralNumberToken)token).Value);
		}

		[AssertionMethod]
		public static void TokenIsLiteralString(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, string value)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(LiteralStringToken));
			Assert.AreEqual(value, ((LiteralStringToken)token).Value);
		}

		[AssertionMethod]
		public static void TokenIsLiteralKeyword(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, bool? value)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(LiteralKeywordToken));
			Assert.AreEqual(value, ((LiteralKeywordToken)token).Value);
		}

		[AssertionMethod]
		public static void TokenIsParentases(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, char expectedOpeningChar)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(PunctuatorToken));
			Assert.AreEqual(PunctuatorToken.Type.OpeningParentases, ((PunctuatorToken)token).PunctuatorType, "First token in group isn't marked as parentases group opening.");
			Assert.AreEqual(expectedOpeningChar, ((PunctuatorToken)token).Character, "Parentases group doesn't start with expected parentases.");

			//Token last = token.LastOrDefault();
			//Assert.IsNotNull(last, "Parentases group doesn't contain any tokens.");
			//Assert.IsInstanceOfType(last, typeof(Punctuator), "Parentases group doesn't end with punctuator.");
			//Assert.AreEqual(((Punctuator)last).PunctuatorType, Punctuator.Type.ClosingParentases, "Parentases group doesn't end with a closing punctuator.");
			//Assert.AreEqual(((Punctuator)last).Character, Punctuator.GetMatchingParentases(expectedOpeningChar), "Parentases group doesn't end with a closing punctuator of correct type.");
		}

		[AssertionMethod]
		public static void TokenIsParentases(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, char expectedOpeningChar, int expectedContainedTokens)
		{
			Assert.That.TokenIsParentases(token, expectedOpeningChar);
			// -1 to exclude the closing parentases
			Assert.AreEqual(expectedContainedTokens, ((PunctuatorToken)token).Count, "Wrong number of direct children for parentases group.");
		}

		[AssertionMethod]
		public static void TokenIsDotOperation(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
			Token token)
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(PunctuatorToken));
			Assert.AreEqual(PunctuatorToken.Type.Dot, ((PunctuatorToken)token).PunctuatorType);
			Assert.AreEqual('.', ((PunctuatorToken)token).Character);
		}

		[AssertionMethod]
		public static void TokenIsOfType<T>(this Assert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token token, string expectedSource)
			where T : Token
		{
			Assert.IsNotNull(token);
			Assert.IsInstanceOfType(token, typeof(T));
			Assert.AreEqual(expectedSource, token.SourceCode, "Incorrect token source code.");
		}

		private static void TokensAreParsed(
			IReadOnlyList<Token> tokens, string listSuffix = "")
		{
			Assert.IsNotNull(tokens, $"Tokens list{listSuffix} is null.");
			for (var i = 0; i < tokens.Count; i++)
			{
				Token token = tokens[i];
				if (token == null) continue;
				Assert.IsTrue(token.IsParsed, $"Token {{{token}}} at list{listSuffix}[{i}] is not parsed!");
				TokensAreParsed(token, $"{listSuffix}[{i}]");
			}
		}

		[AssertionMethod]
		public static void TokensAreParsed(this CollectionAssert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] IReadOnlyList<Token> tokens)
		{
			TokensAreParsed(tokens);
		}

		[AssertionMethod]
		public static void TokensAreOfTypes(this CollectionAssert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] IReadOnlyList<Token> tokens, params Type[] types)
		{
			Assert.IsNotNull(tokens, "Tokens list is null.");
			Assert.AreEqual(types.Length, tokens.Count, "Wrong token count in result.");

			CollectionAssert.AllItemsAreNotNull(tokens.ToList());
			
			for (var i = 0; i < types.Length; i++)
			{
				Assert.IsInstanceOfType(tokens[i], types[i], $"tokens[{i}] wrong type.");
			}
		}

		[AssertionMethod]
		public static void TokensAreSameType(this CollectionAssert assert,
			[AssertionCondition(AssertionConditionType.IS_NOT_NULL)] Token[] tokens, Type type)
		{
			Assert.IsNotNull(tokens, "Tokens list is null.");
			CollectionAssert.AllItemsAreNotNull(tokens);
			
			for (var i = 0; i < tokens.Length; i++)
			{
				Assert.IsInstanceOfType(tokens[i], type, $"tokens[{i}] wrong type.");
			}
		}

		public static void TokenizeAndAssert(IEnumerable<string> samples, params Type[] expected)
		{
			foreach (string input in samples)
			{
				try
				{
					// Act
					Token[] result = Tokenizer.Tokenize(input);

					// Assert
					CollectionAssert.That.TokensAreOfTypes(result, expected);
				}
				catch (ParseException e)
				{
					throw new AssertFailedException($"Failed on:<{input}>. {e.Message}", e);
				}
			}
		}

        [AssertionMethod]
	    public static void AreCodeEqual(this Assert assert, 
	        [CanBeNull] string expected, [CanBeNull] string actual)
        {
            if (actual == expected) return;
            if (expected == null && actual != null)
                throw new AssertFailedException($"Failed code assertion. Expected: <null> Actual:\n```\n{FormatCodeLines(actual.Split('\n'))}\n```");
            if (expected != null && actual == null)
                throw new AssertFailedException($"Failed code assertion. Expected:\n```\n{FormatCodeLines(expected.Split('\n'))}\n```\nActual: <null>");

            string[] expectedLines = expected.Split('\n');
            string[] actualLines = actual.Split('\n');
            for (var line = 0; line < expectedLines.Length; line++)
            {
                if (line >= actualLines.Length)
                    throw new AssertFailedException(
                        $"Failed code assertion. Too few lines.\nExpected {expectedLines.Length} lines:\n```\n{actual}\n```\nActually {actualLines.Length} lines:\n```\n{actual}\n```");

                if (expectedLines[line] != actualLines[line])
                {
                    var point = new Point(expectedLines[line].IndexOfFirstMismatch(actualLines[line]), line);
                    throw new AssertFailedException(
                        $"Failed code assertion. Mismatch on line {line+1}.\nExpected <{expectedLines[line][point.X]}>:\n```\n{FormatCodeLines(expectedLines, point)}\n```\nActual <{actualLines[line][point.X]}>:\n```\n{FormatCodeLines(actualLines, point)}\n```");
                }
            }

            if (expectedLines.Length > actualLines.Length)
                throw new AssertFailedException(
                    $"Failed code assertion. Too many lines.\nExpected {expectedLines.Length} lines:\n```\n{actual}\n```\nActually {actualLines.Length} lines:\n```\n{actual}\n```");

            string FormatCodeLines(string[] codeLines, Point? pointOut = null)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < codeLines.Length; i++)
                {
                    if (sb.Length > 0) sb.Append('\n');
                    string prefix = $"[{i + 1}] ";
                    sb.Append(prefix);
                    sb.Append(codeLines[i]);

                    if (pointOut?.Y == i)
                        sb.Append($"\n{new string(' ', prefix.Length + pointOut.Value.X)}^");
                }

                return sb.ToString();
            }
        }
		
	}
}