using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
	public class SimpleOperatorTests
	{
		[TestMethod]
		public void Tokenize_OneEqual()
		{
			// Arrange
			const string input = "=";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(Operator));
		}

		[TestMethod]
		public void Tokenize_EqualPlus()
		{
			// Arrange
			const string input = "=+";

			// Act
			Token[] result = Tokenizer.Tokenize(input);

			// Assert
			CollectionAssert.That.TokensAreOfTypes(result,
				typeof(Operator),
				typeof(Operator));
		}

		[TestMethod]
		public void Tokenize_UnaryOperators()
		{
			// Arrange
			string[] samples = {
				"+x", "-x", "!x", "~x", "++x", "--x"
			};

			// Act & assert
			Utility.TokenizeAndAssert(samples,
				typeof(Operator),
				typeof(Identifier));
		}

		[TestMethod]
		public void Tokenize_MathOperators()
		{
			// Arrange
			string[] samples = {
				"x*y", "x/y", "x%y",	// multiplicative
				"x+y", "x-y"			// additive
			};

			// Act & assert
			Utility.TokenizeAndAssert(samples,
				typeof(Identifier),
				typeof(Operator),
				typeof(Identifier));
		}


		[TestMethod]
		public void Tokenize_BinaryOperators()
		{
			// Arrange
			string[] samples = {
				"x<<y", "x>>y",		// shift
				"x&y", "x^y", "x|y", "x&y"
			};

			// Act & assert
			Utility.TokenizeAndAssert(samples,
				typeof(Identifier),
				typeof(Operator),
				typeof(Identifier));
		}

		[TestMethod]
		public void Tokenize_BooleanOperators()
		{
			// Arrange
			string[] samples = {
				"x||y", "x&&y"
			};

			// Act & assert
			Utility.TokenizeAndAssert(samples,
				typeof(Identifier),
				typeof(Operator),
				typeof(Identifier));
		}

		[TestMethod]
		public void Tokenize_CompareOperators()
		{
			// Arrange
			string[] samples = {
				"x<y", "x>y", "x<=y", "x>=y",	// relational
				"x==y", "x!=y",					// equality
			};

			// Act & assert
			Utility.TokenizeAndAssert(samples,
				typeof(Identifier),
				typeof(Operator),
				typeof(Identifier));
		}

		[TestMethod]
		public void Tokenize_AssignmentOperators()
		{
			// Arrange
			string[] samples = {
				"x=y",
				"x+=y", "x-=y",
				"x*=y", "x/=y",
				"x%=y", "x&=y",
				"x|=y", "x^=y",
				"x<<=y", "x>>=y",
			};

			// Act & assert
			Utility.TokenizeAndAssert(samples,
				typeof(Identifier),
				typeof(Operator),
				typeof(Identifier));
		}


	}
}