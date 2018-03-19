﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RobotPlusPlus.Linguist.Parsing;
using RobotPlusPlus.Linguist.Tokenizing;
using RobotPlusPlus.Linguist.Tokenizing.Tokens;
using RobotPlusPlus.Linguist.Utility;

namespace RobotPlusPlus.Linguist.Compiling
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
			parsedTokens.AnyRecursive(t =>
			{
				if (t == null)
					throw new NullReferenceException("Token is null!");
				if (!t.IsParsed)
					throw new ApplicationException($"Unparsed token <{t.SourceCode}> on line {t.SourceLine}!");

				return false;
			});

			var compiler = new Compiler();
			var rows = new List<string>(parsedTokens.Length);

			foreach (Token token in parsedTokens)
			{
				compiler.assignmentNeedsCSSnipper = false;
				rows.Add(token.CompileToken(compiler));
			}

			compiler.output.AppendJoin('\n', rows.Where(r => !string.IsNullOrEmpty(r)));

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