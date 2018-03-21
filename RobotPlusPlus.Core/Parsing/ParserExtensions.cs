using System.Collections;
using System.Collections.Generic;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Parsing
{
	public static class ParserExtensions
	{
		public static void ParseTokenAt(this IList<Token> parent, int index)
		{
			Token token = parent[index];
			if (token == null) return;

			if (token.IsParsed) return;
			token.ParseToken(parent, index);
			token.IsParsed = true;
		}
	}
}