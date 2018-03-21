using System.Collections.Generic;
using System.Text.RegularExpressions;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens.Literals
{
	public class LiteralString : Literal
	{
		public string Value { get; }

		public LiteralString(TokenSource source) : base(source)
		{
			Value = Regex.Unescape(SourceCode.Substring(1, SourceCode.Length - 2));
		}

		public override void ParseToken(IList<Token> parent, int myIndex)
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