using RobotPlusPlus.Linguist.Compiling;
using RobotPlusPlus.Linguist.Exceptions;
using RobotPlusPlus.Linguist.Parsing;

namespace RobotPlusPlus.Linguist.Tokenizing.Tokens
{
	/// <summary>Variables. Ex: x, myValue, go_johnny_go</summary>
	public class Identifier : Token
	{
		public Identifier(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{ }

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			if (!compiler.IsVariableRegistered(this))
				throw new UnassignedVariableException(this);
			
			return $"♥{SourceCode}";
		}
	}
}