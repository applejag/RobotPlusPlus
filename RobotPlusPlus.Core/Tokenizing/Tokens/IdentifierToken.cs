using System.Collections.Generic;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Variables. Ex: x, myValue, go_johnny_go</summary>
	public class IdentifierToken : Token
	{
		public IdentifierToken(TokenSource source) : base(source)
		{ }

		public override void ParseToken(IteratedList<Token> parent)
		{ }
	}
}