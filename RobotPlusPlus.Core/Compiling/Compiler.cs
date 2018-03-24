using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling
{
	public class Compiler
	{
		private readonly List<CodeUnit> codeUnits;

		public Compiler()
		{
			codeUnits = new List<CodeUnit>();
		}

		public void AddTokens([ItemNotNull, NotNull] Token[] parsedTokens)
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

			// Convert to code units
			codeUnits.AddRange(parsedTokens
				.Select(CodeUnit.CompileParsedToken)
				.Where(unit => unit != null)
			);
		}

		public void Compile()
		{
			foreach (CodeUnit unit in codeUnits)
			{
				foreach (CodeUnit pre in unit.PreUnits)
					pre.Compile(this);

				unit.Compile(this);

				foreach (CodeUnit post in unit.PostUnits)
					post.Compile(this);
			}
		}

		public string AssembleIntoString()
		{
			if (codeUnits == null)
				return string.Empty;

			// Assemble into code rows
			var rows = new List<string>();

			foreach (CodeUnit unit in codeUnits)
			{
				foreach (CodeUnit pre in unit.PreUnits)
					rows.Add(pre.AssembleIntoString());

				rows.Add(unit.AssembleIntoString());

				foreach (CodeUnit post in unit.PostUnits)
					rows.Add(post.AssembleIntoString());
			}

			return string.Join('\n', rows);
		}

		public static string Compile([ItemNotNull, NotNull] Token[] parsedTokens)
		{
			var compiler = new Compiler();

			compiler.AddTokens(parsedTokens);
			compiler.Compile();

			return compiler.AssembleIntoString();
		}

		public static string Compile([NotNull] string code, [CallerMemberName] string sourceName = "")
		{
			if (code == null)
				throw new ArgumentNullException(nameof(code));

			return Compile(Parser.Parse(Tokenizer.Tokenize(code, sourceName)));
		}
	}
}