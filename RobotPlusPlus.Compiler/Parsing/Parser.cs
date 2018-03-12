using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Parsing
{
	public class Parser
	{
		private readonly List<Token> tokens;
		private int index;

		public Token NextToken => TryGetToken(1);
		public Token CurrToken => TryGetToken(0);
		public Token PrevToken => TryGetToken(-1);

		public bool IsParsingComplete => CurrToken == null;

		private Parser([NotNull, ItemNotNull] IEnumerable<Token> tokens)
		{
			// Ignore whitespace and comments
			//this.tokens = new Queue<Token>(tokens
			//	.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment));

			this.tokens = tokens.ToList();
		}

		public Token TryGetToken(int offset)
		{
			offset = offset + index;
			if (offset >= 0 && offset < tokens.Count)
				return tokens[offset];
			return null;
		}

		public void ParseToken(Token token)
		{
			int i = tokens.IndexOf(token);
			if (i == -1) throw new ArgumentOutOfRangeException(nameof(token), "Token doesn't exist in this asserter!");

			token.ParseToken(this);
		}

		public void Iterate()
		{
			if (IsParsingComplete) throw new InvalidOperationException("The Asserter has already finished!");
			//if (ParseException != null) throw new InvalidOperationException("The Asserter has already failed!", ParseException);

			ParseToken(CurrToken);

			index++;
		}

		public static Token[] Assert([NotNull, ItemNotNull] Token[] tokens)
		{
			var asserter = new Parser(tokens);

			while (!asserter.IsParsingComplete)
				asserter.Iterate();

			return asserter.tokens.ToArray();
		}
	}
}