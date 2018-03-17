using System.Text.RegularExpressions;
using RobotPlusPlus.Linguist.Compiling;
using RobotPlusPlus.Linguist.Parsing;
using RobotPlusPlus.Linguist.Utility;

namespace RobotPlusPlus.Linguist.Tokenizing.Tokens.Literals
{
	public class LiteralString : Literal
	{
		public string Value { get; }

		public LiteralString(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			Value = Regex.Unescape(sourceCode.Substring(1, sourceCode.Length - 2));
		}

		public override void ParseToken(Parser parser)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			string escaped = Value.EscapeString();
			if (escaped == Value && !compiler.assignmentNeedsCSSnipper)
				return $"‴{Value}‴";

			compiler.assignmentNeedsCSSnipper = true;
			return $"\"{escaped}\"";
		}
	}
}