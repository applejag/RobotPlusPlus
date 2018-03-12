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
		private int index;

		public Token NextToken => TryGetToken(1);
		public Token CurrToken => TryGetToken(0);
		public Token PrevToken => TryGetToken(-1);

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

		private Token TakeTokenAt(int i)
		{
			if (i == index)
				throw new InvalidOperationException($"Taking the current token is not allowed (index <{index}>).");
			if (i < 0 || i >= tokens.Count)
				throw new IndexOutOfRangeException($"Token at offset <{i-index}> (index <{i}>) does not exist!");

			Token token = tokens[i];
			tokens.RemoveAt(i);

			if (i > index)
				FinishIteration(index + 1);

			else if (i < index)
				index--;

			return token;
		}

		public Token TakePrevToken()
		{
			return TakeTokenAt(index-1);
		}

		public Token TakeNextToken()
		{
			return TakeTokenAt(index+1);
		}

		protected void ContinueIteration(int start, Predicate<Token> filter)
		{
			for (int i = start; i < tokens.Count; i++)
			{
				Token current = CurrToken;
				if (current.IsParsed)
					continue;

				if (filter == null || filter(current))
				{
					current.IsParsed = true;
					current.ParseToken(this);
				}
			}
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
			for (index = 0; index < tokens.Count; index++)
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