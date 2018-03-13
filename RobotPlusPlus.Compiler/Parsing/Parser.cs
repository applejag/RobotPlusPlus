using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Parsing
{
	public class Parser
	{
		private readonly List<Token> tokens;
		private readonly List<int> indexes = new List<int>();
		private int topIndex => indexes[indexes.Count - 1];
		private readonly HashSet<Token> currentlyParsing = new HashSet<Token>();

		public Token NextToken => TryPeekToken(1);
		public Token CurrToken => TryPeekToken(0);
		public Token PrevToken => TryPeekToken(-1);


		private Parser([NotNull, ItemNotNull] IEnumerable<Token> tokens)
		{
			// Ignore whitespace and comments
			//this.tokens = new Queue<Token>(tokens
			//	.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment));

			this.tokens = tokens.ToList();
		}

		private struct TokenInList
		{
			public readonly IList<Token> parent;
			public readonly Token token;
			public readonly int index;

			public TokenInList(IList<Token> parent, int index)
			{
				this.parent = parent;
				this.index = index;
				token = parent[index];
			}

			public override string ToString()
			{
				return $"parent{{{string.Join(", ", parent)}}} list[{index}]: {token?.ToString() ?? "null"}";
			}
		}

		private List<TokenInList> FlattenTokensList(int direction)
		{
			var list = new List<TokenInList>();

			void AddRangeToList(IList<Token> tokensList, int start, int stop)
			{
				for (int i = start; i < stop; i++)
				{
					if (tokensList[i] == null) continue;
					list.Add(new TokenInList(tokensList, i));
					AddRangeToList(tokensList[i], 0, tokensList[i].Count);
				}
			}

			if (direction > 0)
			{
				AddRangeToList(tokens, start: topIndex + 1, stop: tokens.Count);
			}
			else if (direction < 0)
			{
				AddRangeToList(tokens, start: 0, stop: topIndex);
			}
			else
				list.Add(new TokenInList(tokens, topIndex));

			return list;
		}

		public Token TryPeekToken(int offset)
		{
			List<TokenInList> list = FlattenTokensList(offset);

			if (offset < 0)
				offset = list.Count + offset;
			else if (offset > 0)
				offset--;
			
			if (offset >= 0 && offset < list.Count)
				return list[offset].token;

			return null;
		}

		private Token TakeToken(int offset)
		{
			List<TokenInList> list = FlattenTokensList(offset);
			int index = offset < 0 ? list.Count + offset : offset - 1;

			if (offset == 0)
				throw new InvalidOperationException($"Taking the current token is not allowed (offset <{offset}>).");

			if (index < 0 || index >= list.Count)
				throw new IndexOutOfRangeException($"Token at offset <{offset}> (topIndex <{topIndex}>) does not exist!");

			TokenInList tokenInList = list[index];

			if (ReferenceEquals(tokenInList.parent, tokens))
			{
				// Parse the token
				ParseTokenAt(tokenInList.index);

				// Move all indexes so we don't mess up the iterations
				for (var j = 0; j < indexes.Count; j++)
				{
					// Token that disappeared was before it's counter?
					if (tokenInList.index < indexes[j])
						indexes[j]--;
				}
			}

			// Delete from parent
			tokenInList.parent.RemoveAt(tokenInList.index);

			if (offset > 0)
			{
				// Parse the following before proceeding
				FinishIteration(topIndex + 1);
			}

			return tokenInList.token;
		}

		public Token TakePrevToken()
		{
			return TakeToken(-1);
		}

		public Token TakeNextToken()
		{
			return TakeToken(+1);
		}

		public bool IsTokenParsing(Token token)
		{
			return currentlyParsing.Contains(token);
		}

		public bool IsTokenParsing(Func<Token, bool> predicate)
		{
			return currentlyParsing.Any(predicate);
		}

		public bool IsTokenParsing<T>(Func<T, bool> predicate) where T : Token
		{
			return currentlyParsing.OfType<T>().Any(predicate);
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

		protected void ParseTokenAt(int index)
		{
			int ii = indexes.Count;
			indexes.Add(index);

			ParseSingleToken();

			indexes.RemoveAt(ii);
		}

		protected void ParseSingleToken(Predicate<Token> filter = null)
		{
			Token current = CurrToken;
			if (current.IsParsed || IsTokenParsing(current))
				return;

			if (filter?.Invoke(current) == false) return;

			current.IsParsed = true;
			currentlyParsing.Add(current);
			current.ParseToken(this);
			currentlyParsing.Remove(current);
		}

		protected void ParseTokens(int start, Predicate<Token> filter = null)
		{
			LoopTokens(start, i =>
			{
				ParseSingleToken(filter);
			});
		}

		protected void ParseTokens<T>(int start) where T : Token
		{
			ParseTokens(start, t => t is T);
		}

		protected void ParseTokens<T>(int start, Predicate<T> filter) where T : Token
		{
			ParseTokens(start, t => t is T f && filter(f));
		}

		protected void FinishIteration(int start)
		{
			ParseTokens<Punctuator>(start);

			foreach (Operator.Type type in Enum.GetValues(typeof(Operator.Type)))
			{
				ParseTokens<Operator>(start, o => o.OperatorType == type);
			}

			ParseTokens(start);
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