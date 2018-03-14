using System.Collections.Generic;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	/// <summary>Reserved words. Ex: if, while, try</summary>
	public class Statement : Token
	{
		public Token Condition => Tokens[_Condition];
		public Token CodeBlock => Tokens[_CodeBlock];

		public const int _Condition = 0;
		public const int _CodeBlock = 1;

		public Statement(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{ }

		public override void ParseToken(Parser parser)
		{
			parser.TakeNextToken(_Condition);
			parser.TakeNextToken(_CodeBlock);
		}
	}
}