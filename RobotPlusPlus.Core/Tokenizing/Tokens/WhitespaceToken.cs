using System.Collections.Generic;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Spaces and newlines. Ex: \n, \t, \r</summary>
	public class WhitespaceToken : Token
	{
		public WhitespaceToken(TokenSource source) : base(source)
		{}

		public override void ParseToken(IteratedList<Token> parent)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return string.Empty;
		}
	}
}