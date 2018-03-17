using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;
using RobotPlusPlus.Linguist.Utility;

namespace RobotPlusPlus.Linguist.Parsing
{
	public class Parser
	{
		private readonly List<Token> tokens;
		private int currentTokenIndex = 0;
		private readonly HashSet<Token> currentlyParsing;

		private readonly Parser parent;

		public Token NextToken => TryPeekToken(1);
		public Token CurrToken => TryPeekToken(0);
		public Token PrevToken => TryPeekToken(-1);
		public Parser Root => parent?.Root ?? this;

		private Parser([NotNull, ItemNotNull] IEnumerable<Token> tokens)
		{
			this.tokens = tokens.ToList();
			currentlyParsing = new HashSet<Token>();
		}

		private Parser([NotNull] Parser parent, [NotNull, ItemNotNull] IEnumerable<Token> tokens)
		{
			this.parent = parent;
			this.tokens = tokens.ToList();
			currentlyParsing = parent.currentlyParsing;
		}

		public override string ToString()
		{
			return $"parser{{{string.Join(", ", tokens)}}} list[{currentTokenIndex}]: {CurrToken?.ToString() ?? "null"}";
		}

		public (Parser parent, int index) TryAddressToken(int offset)
		{
			int i = currentTokenIndex + offset;

			// Peek mine
			if (i >= 0 && i < tokens.Count)
				return (this, i);

			// Peek parents next
			if (i >= tokens.Count && parent != null)
				return parent.TryAddressToken(parent.currentTokenIndex + i - tokens.Count + 1);

			// Peek parents prev
			if (i < 0 && parent != null)
				return parent.TryAddressToken(parent.currentTokenIndex + i + 1);

			return (null, default);
		}

		public Token TryPeekToken(int offset)
		{
			(Parser parser, int index) = TryAddressToken(offset);
			return parser?.tokens[index];
		}

		private Token TakeToken(int offset, int insertionIndex)
		{
			(Parser parser, int index) = TryAddressToken(offset);
			Token current = CurrToken;

			if (parser == null)
				throw new IndexOutOfRangeException($"Token at offset <{offset}> does not exist!");

			if (parser.tokens[index] == null)
				throw new NullReferenceException($"Parsers token is null!");

			if (parser == this && parser.currentTokenIndex == index)
				throw new InvalidOperationException($"Taking the current token is not allowed (offset <{offset}>).");

			// Parse the token
			parser.ParseTokenAt(index);

			if (index <= parser.currentTokenIndex)
			{
				// Change index to not mess up iterations
				parser.currentTokenIndex--;
			}

			if (offset > 0)
			{
				// Parse the following before proceeding
				ParseAllFollowingTokens();
			}

			// Transfer
			Token target = parser.tokens[index];
			current.Insert(insertionIndex, target);
			parser.tokens.RemoveAt(index);

			return target;
		}

		public void AddTokensAfterAndParse(params Token[] unparsedTokens)
		{
			tokens.InsertRange(currentTokenIndex + 1, unparsedTokens);
			ParseAllFollowingTokens();
		}

		protected void ParseAllFollowingTokens()
		{
			var parser = new Parser(this, tokens.PopRangeAfter(currentTokenIndex, false));
			parser.ParseAllTokenTypes();
			tokens.AddRange(parser.tokens);
		}

		public void ParseTokenAt(int index)
		{
			new Parser(this, new[] {tokens[index] }).ParseAllTokenTypes();
		}

		public Token TakePrevToken(int insertionIndex)
		{
			return TakeToken(-1, insertionIndex);
		}

		public Token TakePrevToken()
		{
			return TakePrevToken(CurrToken.Count);
		}

		public Token TakeNextToken(int insertionIndex)
		{
			return TakeToken(+1, insertionIndex);
		}

		public Token TakeNextToken()
		{
			return TakeNextToken(CurrToken.Count);
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

		protected void ParseCurrentToken(Predicate<Token> filter = null)
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

		protected void ParseTokens(Predicate<Token> filter = null)
		{
			for (currentTokenIndex = 0; currentTokenIndex < tokens.Count; currentTokenIndex++)
			{
				ParseCurrentToken(filter);		
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
			tokens.RemoveAll(t => t is Comment);
		}

		protected void KillWhitespaces()
		{
			for (currentTokenIndex = 0; currentTokenIndex < tokens.Count; currentTokenIndex++)
			{
				if (CurrToken is Whitespace) continue;

				if (PrevToken is Whitespace leading)
					CurrToken.TrailingWhitespace = leading;

				if (NextToken is Whitespace trailing)
					CurrToken.TrailingWhitespace = trailing;
			}

			tokens.RemoveAll(t => t is Whitespace);
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
			return parser.tokens.ToArray();
		}

	}
}