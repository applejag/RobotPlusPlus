using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using RobotPlusPlus.Parsing;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Compiling
{
	public class Compiler
	{
		private readonly StringBuilder output = new StringBuilder();

		public static string Compile([ItemNotNull, NotNull] Token[] parsedTokens)
		{
			if (parsedTokens == null)
				throw new ArgumentNullException(nameof(parsedTokens));

			var compiler = new Compiler();

			foreach (Token token in parsedTokens)
			{
				if (compiler.output.Length > 0)
					compiler.output.AppendLine();

				compiler.output.Append(token.CompileToken());
			}

			return compiler.output.ToString();
		}

		public static string Compile([NotNull] string code)
		{
			if (code == null)
				throw new ArgumentNullException(nameof(code));

			return Compile(Parser.Parse(Tokenizer.Tokenize(code)));
		}
	}
}