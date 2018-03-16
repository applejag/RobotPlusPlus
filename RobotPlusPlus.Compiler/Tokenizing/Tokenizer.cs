using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RobotPlusPlus.Exceptions;
using RobotPlusPlus.Tokenizing.Tokens;
using RobotPlusPlus.Tokenizing.Tokens.Literals;

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

		private static readonly IReadOnlyCollection<string> statements = new[]
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

		private static readonly IReadOnlyCollection<char> punctuators = new[]
		{
			'.', ',', ':', ';', '(', ')', '[', ']', '{', '}', '?'
		};

		private static readonly IReadOnlyCollection<string> operators = new[]
		{
			// Comparisons
			"==", "<=", ">=", "!=",
			// Assignment
			"=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
			// Math
			"++", "--", "+", "-", "*", "/", "%",
			// Boolean
			"&&", "||", "!",
			// Binary
			"&", "|", "^", "<<", ">>", "~",
			// Comparisons 2
			"<", ">",
		};

		private Tokenizer(string sourceCode)
		{
			SourceCode = sourceCode;
			remainingCode = sourceCode;
		}

		private Token EvaluateNextType()
		{
			string segment;

			// Block comment
			if ((segment = MatchingRegex(@"\/\*(\*(?!\/)|[^*])*\*\/")) != null)
				return new Comment(segment, CurrentRow, isBlock: true);

			// Incomplete block comment
			if (MatchingRegex(@"\/\*(\*(?!\/)|[^*])*\z") != null)
				throw new ParseException("Nonterminated block comment.", CurrentRow);
			
			// Singleline comment
			if ((segment = MatchingRegex(@"\/\/.*")) != null)
				return new Comment(segment, CurrentRow, isBlock: false);

			// Whitespace
			if ((segment = MatchingRegex(@"\s+")) != null)
				return new Whitespace(segment, CurrentRow);

			// Literal keywords
			if ((segment = MatchingWordsInList(literalKeywords, ignoreCase: false)) != null)
				return new LiteralKeyword(segment, CurrentRow);

			// Statements
			if ((segment = MatchingWordsInList(statements, ignoreCase: false)) != null)
				return new Statement(segment, CurrentRow);

			// Identifiers
			if ((segment = MatchingRegex(@"[\p{L}_][\p{L}_\p{N}]*")) != null)
				return new Identifier(segment, CurrentRow);

			// Strings
			foreach (char s in strings)
			{
				// Full string
				if ((segment = MatchingRegex($@"{s}([^{s}\\]|\\.)*{s}")) != null)
					return new LiteralString(segment, CurrentRow);
				
				// Incomplete '' strings
				if (MatchingRegex($@"{s}([^{s}\\]|\\.)*\z") != null)
					throw new ParseException("Nonterminated string literal.", CurrentRow);
			}

			// Numbers
			if ((segment = MatchingRegex(@"(\d+\.?\d*|\d*\.\d+)[fF]?")) != null)
			{
				// Ending with invalid char?
				if (MatchingRegex(@"(\d+\.?\d*|\d*\.\d+)[\p{L}_]*") != segment)
					throw new ParseException("Unexpected character after number.", CurrentRow);

				return new LiteralNumber(segment, CurrentRow);
			}

			// Punctuators
			if (punctuators.Contains(remainingCode[0]))
				return new Punctuator(remainingCode.Substring(0, 1), CurrentRow);

			// Operators
			if ((segment = MatchingStringInList(operators)) != null)
				return new Operator(segment, CurrentRow);

			// Unknown
			throw new ParseException("Unable to parse next token.", CurrentRow);
		}
		
		public void Iterate()
		{
			if (IsParsingComplete) throw new InvalidOperationException("The Tokenizer has already finished!");
			if (ParseException != null) throw new InvalidOperationException("The Tokenizer has already failed!", ParseException);

			try
			{
				Token token = EvaluateNextType();
				tokens.Add(token);

				// Update remaining code
				remainingCode = remainingCode.Substring(token.SourceCode.Length);

				// Update row
				foreach (char c in token.SourceCode)
				{
					if (c == '\n')
						CurrentRow++;
				}
			}
			catch (ParseException e)
			{
				ParseException = e;
				throw;
			}
			catch (Exception e)
			{
				ParseException = new ParseException("Unkown exception during parsing!", CurrentRow, e);
				throw ParseException;
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
			}

			return tokenizer.Tokens.ToArray();
		}

		#region Comparators

		public string MatchingRegex(
			[RegexPattern, NotNull] string pattern,
			RegexOptions options = RegexOptions.IgnoreCase)
		{
			Match match = Regex.Match(remainingCode, $@"^{pattern}", options);

			return match.Success && match.Length > 0
				? remainingCode.Substring(0, match.Length)
				: null;
		}

		public string MatchingWordsInList(
			[NotNull, ItemNotNull] IReadOnlyCollection<string> words,
			bool ignoreCase = false)
		{
			string match = Regex.Match(remainingCode, @"^\w*").Value;
			StringComparison option = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

			return words.FirstOrDefault(word => match.Equals(word, option));
		}

		public string MatchingStringInList(
			[NotNull, ItemNotNull] IReadOnlyCollection<string> samples,
			bool ignoreCase = false)
		{
			StringComparison option = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

			foreach (string word in samples)
			{
				if (remainingCode.Length < word.Length) continue;

				string match = remainingCode.Substring(0, word.Length);

				if (match.Equals(word, option))
				{
					return word;
				}
			}

			return null;
		}

		#endregion
	}
}