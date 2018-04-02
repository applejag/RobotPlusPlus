using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Parsing
{
	public class Parser
	{
		private List<Token> Tokens { get; }

		private Parser([NotNull, ItemNotNull] IEnumerable<Token> tokens)
		{
			Tokens = tokens.ToList();
		}

		public override string ToString()
		{
			return $"parser{{{string.Join(", ", Tokens)}}}";
		}

		protected void ParseTokens(Predicate<Token> filter = null, bool reversed = false)
		{
			ParseTokens(Tokens, filter, reversed);
		}

		private static void ParseTokens(IList<Token> tokens, Predicate<Token> filter, bool reversed)
		{
			var parent = new IteratedList<Token>(tokens, reversed);

			bool any;
			do
			{
				any = false;
				foreach (Token token in parent)
				{
					if (token is null) continue;

					ParseTokens(parent.Current, filter, parent.Reversed);

					if (token.IsParsed) continue;
					if (filter != null && !filter.Invoke(token)) continue;

					token.ParseToken(parent);
					token.IsParsed = true;
					any = true;
				}

			} while (any);
		}

		protected void ParseAllTokenTypes()
		{
			ParseTokens(token => token is PunctuatorToken);

			foreach (OperatorToken.Type type in Enum.GetValues(typeof(OperatorToken.Type)))
			{
				ParseTokens(token => token is OperatorToken o && o.OperatorType == type,
					reversed: type == OperatorToken.Type.Assignment);
			}

			ParseTokens();
		}

		protected void KillComments()
		{
			Tokens.RemoveAll(t => t is CommentToken);
		}

		protected void KillWhitespaces()
		{
			for (var i= 0; i < Tokens.Count; i++)
			{
				Token CurrToken = Tokens[i];
				if (CurrToken is WhitespaceToken) continue;

				if (Tokens.TryGet(i-1) is WhitespaceToken leading)
					CurrToken.TrailingWhitespaceToken = leading;

				if (Tokens.TryGet(i+1) is WhitespaceToken trailing)
					CurrToken.TrailingWhitespaceToken = trailing;
			}

			Tokens.RemoveAll(t => t is WhitespaceToken);
		}

		public static Token[] Parse([ItemNotNull] Token[] tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));

			var parser = new Parser(tokens);

			parser.KillComments();
			parser.KillWhitespaces();

			parser.ParseAllTokenTypes();

			// Kdone
			return parser.Tokens.ToArray();
		}

	}
}