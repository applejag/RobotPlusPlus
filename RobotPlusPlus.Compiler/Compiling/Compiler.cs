using System;
using System.Collections.Generic;
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
		private readonly HashSet<string> registeredVariables = new HashSet<string>();
		
		public bool assignmentNeedsCSSnipper;

		public void RegisterVariable([NotNull] Identifier identifier)
		{
			registeredVariables.Add(identifier.SourceCode);
		}

		public bool IsVariableRegistered([CanBeNull] Identifier identifier)
		{
			return identifier != null && registeredVariables.Contains(identifier.SourceCode);
		}

		public static string Compile([ItemNotNull, NotNull] Token[] parsedTokens)
		{
			if (parsedTokens == null)
				throw new ArgumentNullException(nameof(parsedTokens));

			var compiler = new Compiler();

			foreach (Token token in parsedTokens)
			{
				if (compiler.output.Length > 0)
					compiler.output.Append('\n');

				compiler.assignmentNeedsCSSnipper = false;
				compiler.output.Append(token.CompileToken(compiler));
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