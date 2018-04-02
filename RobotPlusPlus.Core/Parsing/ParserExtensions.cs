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

			for (var i = 0; i < 42; i++)
			{
				token.ParseToken(new IteratedList<Token>(parent, index, parent.Reversed));
				token.IsParsed = true;

				token = index < parent.Count ? parent[index] : null;

				if (token?.IsParsed != false)
					return true;
			}

			throw new ParseTokenException($"Detected loop while parsing token <{token.SourceCode}>.", token);
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