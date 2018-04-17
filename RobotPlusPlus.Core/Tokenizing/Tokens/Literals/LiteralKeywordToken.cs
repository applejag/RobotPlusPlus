using System.Collections.Generic;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens.Literals
{
	public class LiteralKeywordToken : LiteralToken
	{
		public bool IsBool => Value is bool;
		public bool IsNull => Value is null;
		public object Value { get; }

		public LiteralKeywordToken(TokenSource source) : base(source)
		{
			switch (SourceCode)
			{
				case "true":
					Value = true;
					break;

				case "false":
					Value = false;
					break;

				case "null":
					Value = null;
					break;

				default:
					throw new ParseTokenException($"Unkown literal keyword <{SourceCode}>!", this);
			}
		}

		public override void ParseToken(IteratedList<Token> parent)
		{ }
	}
}