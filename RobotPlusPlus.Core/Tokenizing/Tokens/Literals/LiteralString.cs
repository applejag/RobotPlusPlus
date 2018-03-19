using System.Text.RegularExpressions;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens.Literals
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