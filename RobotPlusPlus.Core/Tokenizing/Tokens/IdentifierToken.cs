﻿using System.Collections.Generic;
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

		public override void ParseToken(IList<Token> parent, int myIndex)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			if (!compiler.IsVariableRegistered(this))
				throw new UnassignedVariableException(this);
			
			return $"♥{SourceCode}";
		}
	}
}