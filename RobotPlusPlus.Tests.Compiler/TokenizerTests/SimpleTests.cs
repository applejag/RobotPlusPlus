﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Tests.TokenizerTests
{
	[TestClass]
	public class SimpleTests
	{
		[TestMethod]
		public void Tokenize_Null()
		{
			// Act
			Token[] result = Tokenizer.Tokenize(null);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void Tokenize_Whitespace()
		{
			// Act
			Token[] result = Tokenizer.Tokenize("   	 		  ");

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Length);
		}

	}
}