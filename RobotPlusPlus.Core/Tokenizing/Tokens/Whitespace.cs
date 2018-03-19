using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Spaces and newlines. Ex: \n, \t, \r</summary>
	public class Whitespace : Token
	{
		public Whitespace(TokenSource source) : base(source)
		{}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return string.Empty;
		}
	}
}