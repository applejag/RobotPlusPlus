using System.Collections.Generic;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Variables. Ex: x, myValue, go_johnny_go</summary>
	public class Identifier : Token
	{
		public Identifier(TokenSource source) : base(source)
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