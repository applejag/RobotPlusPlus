using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Parsing
{
	public static class ParserExtensions
	{
		public static bool ParseTokenAt([NotNull] this IteratedList<Token> parent, int index)
		{
			Token token = parent[index];

			if (token == null) return false;

			if (token.IsParsed) return false;
			token.ParseToken(new IteratedList<Token>(parent.List, index, parent.Reversed));
			token.IsParsed = true;

			return true;
		}

		public static bool ParseNextToken([NotNull] this IteratedList<Token> parent)
		{
			return ParseTokenAt(parent, parent.Index + 1);
		}

		public static bool ParsePreviousToken([NotNull] this IteratedList<Token> parent)
		{
			return ParseTokenAt(parent, parent.Index - 1);
		}
	}
}