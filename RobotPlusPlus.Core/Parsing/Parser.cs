using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

		protected void ParseTokens(Predicate<Token> filter = null)
		{
			ParseTokens(Tokens, filter);
		}

		protected static void ParseTokens(IList<Token> tokens, Predicate<Token> filter)
		{
			while (true)
			{
				var any = false;

				for (var i = 0; i < tokens.Count; i++)
				{
					Token token = tokens[i];
					if (token == null) continue;

					ParseTokens(token, filter);
					if (filter != null && !filter.Invoke(token)) continue;

					any |= tokens.ParseTokenAt(i);
				}

				if (!any) return;
			}
		}

		protected void ParseAllTokenTypes()
		{
			ParseTokens(token => token is Punctuator);

			foreach (Operator.Type type in Enum.GetValues(typeof(Operator.Type)))
			{
				ParseTokens(token => token is Operator o && o.OperatorType == type);
			}

			ParseTokens();
		}

		protected void KillComments()
		{
			Tokens.RemoveAll(t => t is Comment);
		}

		protected void KillWhitespaces()
		{
			for (var i= 0; i < Tokens.Count; i++)
			{
				Token CurrToken = Tokens[i];
				if (CurrToken is Whitespace) continue;

				if (Tokens.TryGet(i-1) is Whitespace leading)
					CurrToken.TrailingWhitespace = leading;

				if (Tokens.TryGet(i+1) is Whitespace trailing)
					CurrToken.TrailingWhitespace = trailing;
			}

			Tokens.RemoveAll(t => t is Whitespace);
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