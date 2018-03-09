using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace RobotPlusPlus.Tokenizing
{
	public class Tokenizer
	{
		private string remainingCode;
		private readonly List<Token> tokens = new List<Token>();

		public string SourceCode { get; }
		public IReadOnlyList<Token> Tokens => tokens;
		public int CurrentRow { get; private set; } = 1;
		public bool IsParsingComplete => remainingCode.Length == 0;
		public bool IsParsingSuccessful => ParseException == null;
		public ParseException ParseException { get; private set; } = null;

		private static readonly IReadOnlyCollection<string> keywords = new[]
		{
			"if", "while", "try"
		};

		private static readonly IReadOnlyCollection<char> strings = new[]
		{
			'"', '\''
		};

		private static readonly IReadOnlyCollection<string> literalKeywords = new[]
		{
			"true", "false", "null"
		};

		private Tokenizer(string sourceCode)
		{
			SourceCode = sourceCode;
			remainingCode = sourceCode;
		}

		private (TokenType type, int length) EvaluateNextType()
		{
			int length;

			// Block comment
			if ((length = MatchingRegex(@"\/\*(\*(?!\/)|[^*])*\*\/")) > 0)
				return (TokenType.Comment, length);

			// Incomplete block comment
			if (MatchingRegex(@"\/\*(\*(?!\/)|[^*])*\z") > 0)
				throw new ParseException("Nonterminated block comment.", CurrentRow);

			// Singleline comment
			if ((length = MatchingRegex(@"\/\/.*")) > 0)
				return (TokenType.Comment, length);

			// Whitespace
			if ((length = MatchingRegex(@"\s+")) > 0)
				return (TokenType.Whitespace, length);

			// Literal keywords
			if ((length = MatchingWordsInList(literalKeywords, ignoreCase: false)) > 0)
				return (TokenType.Literal, length);

			// Keywords
			if ((length = MatchingWordsInList(keywords, ignoreCase: false)) > 0)
				return (TokenType.Keyword, length);

			// Identifiers
			if ((length = MatchingRegex(@"[\p{L}_][\p{L}_\p{N}]*")) > 0)
				return (TokenType.Identifier, length);

			// Strings
			foreach (char s in strings)
			{
				// Full string
				if ((length = MatchingRegex($@"{s}([^{s}\\]|\\.)*{s}")) > 0)
					return (TokenType.Literal, length);
				
				// Incomplete '' strings
				if (MatchingRegex($@"{s}([^{s}\\]|\\.)*\z") > 0)
					throw new ParseException("Nonterminated string literal.", CurrentRow);
			}
			
			// Unknown
			throw new ParseException("Unable to parse next token.", CurrentRow);
		}

		public void Iterate()
		{
			if (IsParsingComplete) throw new InvalidOperationException("The Tokenizer has already finished!");
			if (ParseException != null) throw new InvalidOperationException("The Tokenizer has already failed!", ParseException);

			try
			{
				(TokenType type, int length) = EvaluateNextType();

				// Create token
				string tokenSegment = remainingCode.Substring(0, length);
				tokens.Add(new Token(type, tokenSegment));

				// Update remaining code
				remainingCode = remainingCode.Substring(length);

				// Update row
				foreach (char c in tokenSegment)
				{
					if (c == '\n')
						CurrentRow++;
				}
			}
			catch (ParseException e)
			{
				ParseException = e;
			}
		}

		public static Token[] Tokenize(string code)
		{
			if (string.IsNullOrEmpty(code))
				return new Token[0];

			var tokenizer = new Tokenizer(code);

			while (!tokenizer.IsParsingComplete)
			{
				tokenizer.Iterate();

				if (tokenizer.ParseException != null)
					throw tokenizer.ParseException;
			}

			return tokenizer.Tokens.ToArray();
		}

		#region Comparators

		public int MatchingRegex(
			[RegexPattern, NotNull] string pattern,
			RegexOptions options = RegexOptions.IgnoreCase)
		{
			return Regex.Match(remainingCode, $@"^{pattern}", options).Length;
		}

		public int MatchingWordsInList(
			[NotNull, ItemNotNull] IReadOnlyCollection<string> words,
			bool ignoreCase = false)
		{
			string match = Regex.Match(remainingCode, @"^\w*").Value;
			StringComparison option = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

			foreach (string word in words)
			{
				if (match.Equals(word, option))
				{
					return word.Length;
				}
			}

			return 0;
		}

		#endregion
	}
}