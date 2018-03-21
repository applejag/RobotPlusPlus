using System;
using System.Collections;
using System.Collections.Generic;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Parsing
{
	public static class ParserExtensions
	{
		public static bool ParseTokenAt(this IList<Token> parent, int index)
		{
			Token token = parent[index];
			if (token == null) return false;

			if (token.IsParsed) return false;
			token.ParseToken(parent, index);
			token.IsParsed = true;

			return true;
		}
	}
}