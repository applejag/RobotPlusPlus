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
		public bool IsParsingSuccessful => ParsingException == null;
		public Exception ParsingException { get; private set; } = null;

		private Tokenizer(string sourceCode)
		{
			SourceCode = sourceCode;
			remainingCode = sourceCode;
		}

		private (TokenType, int length) EvaluateNextType()
		{

		}

		public void Iterate()
		{
			if (IsParsingComplete) throw new InvalidOperationException("The Tokenizer has already finished!");
			if (ParsingException != null) throw new InvalidOperationException("The Tokenizer has already failed!", ParsingException);

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
			catch (Exception e)
			{
				ParsingException = e;
			}
		}

		public static Token[] Tokenize(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
				return new Token[0];

			var tokenizer = new Tokenizer(code);

			while (!tokenizer.IsParsingComplete)
			{
				tokenizer.Iterate();

				if (tokenizer.ParsingException != null)
					throw tokenizer.ParsingException;
			}

			return tokenizer.Tokens.ToArray();
		}

	}
}