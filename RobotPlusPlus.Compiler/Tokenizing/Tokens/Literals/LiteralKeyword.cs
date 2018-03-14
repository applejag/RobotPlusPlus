using RobotPlusPlus.Compiling;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens.Literals
{
	public class LiteralKeyword : Literal
	{
		public bool IsBool => Value is bool;
		public bool IsNull => Value is null;
		public object Value { get; }

		public LiteralKeyword(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			switch (sourceCode)
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
					throw new ParseException($"Unkown literal keyword <{sourceCode}>!", sourceLine);
			}
		}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return SourceCode;
		}
	}
}