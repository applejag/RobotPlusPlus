using RobotPlusPlus.Tokenizing;
using System;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Asserting.Definitions;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Asserting
{
	public class Asserter
	{
		private readonly Queue<Token> tokens;
		private readonly HashSet<Variable> variablePool = new HashSet<Variable>();

		public bool IsParsingComplete => tokens.Count == 0;
		public bool IsParsingSuccessful => ParseException == null;
		public ParseException ParseException { get; private set; } = null;

		public Token LastToken { get; private set; }
		private Token targetIdentifier;

		public bool AnyMoreTokens => tokens.Count > 0;

		private Asserter(Token[] tokens)
		{
			// Ignore whitespace and comments
			this.tokens = new Queue<Token>(tokens
				.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment));
		}

		private void EvaluateNextDefinition()
		{
			Token token = NextToken();
			if (token == null) return;
			
			if (token.Type == TokenType.Identifier)
			{
				// Two identifier in a row?
				if (targetIdentifier != null)
					throw new ParseException($"Unexpected identifier {token.SourceCode}", token.SourceLine);

				targetIdentifier = token;
				return;
			}
			
			if (token is OperatorToken op && op.OperatorType == OperatorToken.Type.Assignment)
			{
				tokens.Enqueue(token);
				tokens.Enqueue(targetIdentifier);
				targetIdentifier = null;
			}
		}

		public Token NextToken()
		{
			if (tokens.TryDequeue(out Token value))
			{
				LastToken = value;
				return value;
			}
			else
			{
				throw new ParseException("Unexpected EOF!", LastToken.SourceLine);
			}
		}

		public Token PeekToken()
		{
			return tokens.TryPeek(out Token value) ? value : null;
		}

		public void Iterate()
		{
			if (IsParsingComplete) throw new InvalidOperationException("The Asserter has already finished!");
			if (ParseException != null) throw new InvalidOperationException("The Asserter has already failed!", ParseException);
		}

		public static void Assert(Token[] tokens)
		{
			var asserter = new Asserter(tokens);
		}
	}
}