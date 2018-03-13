using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Reserved words. Ex: if, while, try</summary>
	public class Statement : Token
	{
		public Token Condition => Tokens[0];
		public Token CodeBlock => Tokens[1];

		public Statement(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			Tokens = new List<Token> {null, null};
		}

		public override void ParseToken(Parser parser)
		{
			parser.TakeNextToken(0);
			parser.TakeNextToken(1);
		}
	}
}