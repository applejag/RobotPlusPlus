using System.Collections.Generic;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Asserting.Definitions
{
	public class Expression
	{
		public List<Token> tokens = new List<Token>();
		public Token LastToken => tokens.Count > 0 ? tokens[tokens.Count - 1] : null;

		public Expression ReadParentasesGroup(Asserter asserter)
		{
			// Count the parentases
			var parentases = 0;

			// Check at least one open parentases
			{
				Token peekToken = asserter.PeekToken();
				if (peekToken.Type == TokenType.Punctuator && peekToken.SourceCode != "(")
					throw new ParseException($"Expected <(>, got <{peekToken.SourceCode}>", peekToken.SourceLine);
			}

			// Keep pulling
			do
			{
				Token token = asserter.NextToken();
				if (token.Type == TokenType.Punctuator)
				{
					if (token.SourceCode == "(") parentases++;
					else if (token.SourceCode == ")") parentases--;
				}
				tokens.Add(token);

			} while (parentases > 0);

			return this;
		}

		public Expression ReadUntilSemicolon(Asserter asserter)
		{
			// Keep pulling until we get a semicolon
			do
			{
				tokens.Add(asserter.NextToken());
			} while (LastToken.Type != TokenType.Punctuator && LastToken.SourceCode != ";");

			return this;
		}
		
	}
}