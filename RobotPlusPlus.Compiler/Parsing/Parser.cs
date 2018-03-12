using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Parsing
{
	public class Parser
	{
		private readonly List<Token> tokens;

		public Token NextToken => TryGetToken(1);
		public Token CurrToken => TryGetToken(0);
		public Token PrevToken => TryGetToken(-1);

		private readonly List<int> indexes = new List<int>();
		private int topIndex => indexes[indexes.Count - 1];

		private Parser([NotNull, ItemNotNull] IEnumerable<Token> tokens)
		{
			// Ignore whitespace and comments
			//this.tokens = new Queue<Token>(tokens
			//	.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment));

			this.tokens = tokens.ToList();
		}

		public Token TryGetToken(int offset)
		{
			offset = offset + topIndex;
			if (offset >= 0 && offset < tokens.Count)
				return tokens[offset];
			return null;
		}

		private Token TakeTokenAt(int i)
		{
			if (i == topIndex)
				throw new InvalidOperationException($"Taking the current token is not allowed (index <{topIndex}>).");
			if (i < 0 || i >= tokens.Count)
				throw new IndexOutOfRangeException($"Token at offset <{i - topIndex}> (index <{i}>) does not exist!");

			Token token = tokens[i];
			tokens.RemoveAt(i);

			if (i > topIndex)
				FinishIteration(topIndex + 1);

			else
			{
				for (var j = 0; j < indexes.Count; j++)
				{
					if (i < indexes[j])
						indexes[j]--;
				}
			}

			return token;
		}

		public Token TakePrevToken()
		{
			return TakeTokenAt(topIndex - 1);
		}

		public Token TakeNextToken()
		{
			return TakeTokenAt(topIndex + 1);
		}

		protected void LoopTokens(int start, Action<int> callback)
		{

			int ii = indexes.Count;
			indexes.Add(start);

			for (; indexes[ii] < tokens.Count; indexes[ii]++)
			{
				callback(indexes[ii]);
			}

			indexes.RemoveAt(ii);

		}

		protected void ContinueIteration(int start, Predicate<Token> filter)
		{
			LoopTokens(start, i =>
			{

				Token current = CurrToken;
				if (current.IsParsed)
					return;

				if (filter == null || filter(current))
				{
					current.IsParsed = true;
					current.ParseToken(this);
				}
			});
		}

		protected void ContinueIteration<T>(int start) where T : Token
		{
			ContinueIteration(start, t => t is T);
		}

		protected void ContinueIteration<T>(int start, Predicate<T> filter) where T : Token
		{
			ContinueIteration(start, t => t is T f && filter(f));
		}

		protected void FinishIteration(int start)
		{
			ContinueIteration<Punctuator>(start);

			foreach (Operator.Type type in Enum.GetValues(typeof(Operator.Type)))
			{
				ContinueIteration<Operator>(start, o => o.OperatorType == type);
			}

			ContinueIteration(start, null);
		}

		protected void FullIteration()
		{
			FinishIteration(0);
		}

		protected void KillComments()
		{
			tokens.RemoveAll(t => t is Comment);
		}

		protected void KillWhitespaces()
		{
			LoopTokens(0, i =>
			{
				if (CurrToken is Whitespace) return;

				if (PrevToken is Whitespace leading)
					CurrToken.TrailingWhitespace = leading;

				if (NextToken is Whitespace trailing)
					CurrToken.TrailingWhitespace = trailing;
			});

			tokens.RemoveAll(t => t is Whitespace);
		}

		public static Token[] Parse([ItemNotNull] Token[] tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));

			var parser = new Parser(tokens);

			parser.KillComments();
			parser.KillWhitespaces();

			parser.FullIteration();

			// Kdone
			return parser.tokens.ToArray();
		}

		public static Token[] Parse(string code)
		{
			return Tokenizer.Tokenize(code);
		}

		public static string Compile(string code)
		{
			throw new NotImplementedException();
		}

	}
}