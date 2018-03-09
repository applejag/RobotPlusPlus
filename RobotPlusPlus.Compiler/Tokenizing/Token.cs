using System;
using System.Linq;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Tokenizing
{
	public class Token
	{
		public TokenType Type { get; }
		public string Source { get; }

		public Token(TokenType type, string source)
		{
			Type = type;
			Source = source;
		}
		
	}
}