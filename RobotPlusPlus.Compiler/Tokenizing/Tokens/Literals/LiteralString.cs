using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Parsing;
using RobotPlusPlus.Utility;

namespace RobotPlusPlus.Tokenizing.Tokens.Literals
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
			if (escaped != Value)
			{
				compiler.assignmentNeedsCSSnipper = true;
				return $"\"{escaped}\"";
			}

			return $"‴{escaped}‴";
		}
	}
}