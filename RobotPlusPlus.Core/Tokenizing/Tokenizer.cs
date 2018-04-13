using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;

namespace RobotPlusPlus.Core.Tokenizing
{
	public class Tokenizer
	{
		private string remainingCode;
		private readonly List<Token> tokens = new List<Token>();

		public string SourceName { get; }
		public string SourceCode { get; }
		public IReadOnlyList<Token> Tokens => tokens;
		public int CurrentRow { get; private set; } = 1;
		public int CurrentColumn { get; private set; } = 1;
		public bool IsParsingComplete => remainingCode.Length == 0;
		public bool IsParsingSuccessful => ParseException == null;
		public ParseException ParseException { get; private set; } = null;

		private static readonly IReadOnlyCollection<string> statements = new[]
		{
			"if", "else", "while", "do"
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

		private Tokenizer(string sourceCode, string sourceName)
		{
			SourceCode = sourceCode;
			remainingCode = sourceCode;
			SourceName = sourceName;
		}

		private Token EvaluateNextType()
		{
			var source = new TokenSource(string.Empty, SourceName, CurrentRow, CurrentColumn);

			// Block comment
			if ((source.code = MatchingRegex(@"\/\*(\*(?!\/)|[^*])*\*\/")) != null)
				return new CommentToken(source, isBlock: true);

			// Incomplete block comment
			if (MatchingRegex(@"\/\*(\*(?!\/)|[^*])*\z") != null)
				throw new ParseException("Nonterminated block comment.", CurrentRow);
			
			// Singleline comment
			if ((source.code = MatchingRegex(@"\/\/.*")) != null)
				return new CommentToken(source, isBlock: false);

			// Whitespace
			if ((source.code = MatchingRegex(@"\s+")) != null)
				return new WhitespaceToken(source);

			// Literal keywords
			if ((source.code = MatchingWordsInList(literalKeywords, ignoreCase: false)) != null)
				return new LiteralKeywordToken(source);

			// Statements
			if ((source.code = MatchingWordsInList(statements, ignoreCase: false)) != null)
				return new StatementToken(source);

			// Identifiers
			if ((source.code = MatchingRegex(@"[\p{L}_][\p{L}_\p{N}]*")) != null)
				return new IdentifierToken(source);

			// Strings
			foreach (char s in strings)
			{
				// Full string
				if ((source.code = MatchingRegex($@"{s}([^{s}\\]|\\.)*{s}")) != null)
					return new LiteralStringToken(source);
				
				// Incomplete '' strings
				if (MatchingRegex($@"{s}([^{s}\\]|\\.)*\z") != null)
					throw new ParseException("Nonterminated string literal.", CurrentRow);
			}

			// Numbers
			if ((source.code = MatchingRegex(@"(\d+\.?\d*[fF]|\d+\.?(?![\p{L}_])\d*[fF]?|\d*\.\d+[fF]?)")) != null)
			{
				// Ending with invalid char?
				if (MatchingRegex(@"(\d+\.?\d*[fF]|\d+\.?(?![\p{L}_])\d*[fF]?|\d*\.\d+[fF]?)[\p{L}_]*") != source.code)
					throw new ParseException($"Unexpected character(s) after number <{source.code}>.", CurrentRow);
				
				return new LiteralNumberToken(source);
			}

			// Punctuators
			if (punctuators.Contains(remainingCode[0]))
			{
				source.code = remainingCode.Substring(0, 1);
				return new PunctuatorToken(source);
			}

			// Operators
			if ((source.code = MatchingStringInList(operators)) != null)
				return new OperatorToken(source);

			// Unknown
			throw new ParseException($"Unable to parse next token. Next {Math.Min(remainingCode.Length, 32)} characters: <{remainingCode.Substring(0, Math.Min(remainingCode.Length, 32))}>", CurrentRow);
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
					{
						CurrentRow++;
						CurrentColumn = 1;
					}
					else
						CurrentColumn++;
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

		public static Token[] Tokenize(string code, [CallerMemberName] string sourceName = "")
		{
			if (string.IsNullOrEmpty(code))
				return new Token[0];

			if (string.IsNullOrWhiteSpace(sourceName))
				throw new ArgumentException("Source name cannot be null or whitespace!", nameof(sourceName));

			var tokenizer = new Tokenizer(code, sourceName.Trim());

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