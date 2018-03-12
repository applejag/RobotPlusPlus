using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Reserved words. Ex: if, while, try</summary>
	public class Statement : Token
	{
		public Token Condition
		{
			get => Tokens[0];
			set => Tokens[0] = value;
		}

		public Token CodeBlock
		{
			get => Tokens[1];
			set => Tokens[1] = value;
		}

		public Statement(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			Tokens = new List<Token> {null, null};
		}

		public override void ParseToken(Parser parser)
		{
			Condition = parser.TakeNextToken();
			CodeBlock = parser.TakeNextToken();
		}
	}
}