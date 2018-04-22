using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Structures.CSharp;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling
{
	public class Compiler
	{
		private readonly List<CodeUnit> codeUnits;

		public ValueContext Context { get; private set; }

		public G1ANTRepository G1ANTRepository { get; private set; }
		public CSharpRepository CSharpRepository { get; private set; }

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
			// Load predefined values
			G1ANTRepository = G1ANTRepository.FromEmbeddedXML();
			CSharpRepository = new CSharpRepository();

			// Reset contexts
			Context = GetPopulatedContext(G1ANTRepository, CSharpRepository);

			// Compile
			CompileUnits(codeUnits, this);
		}

		public static void CompileUnits(List<CodeUnit> codeUnits, Compiler compiler)
		{
			foreach (CodeUnit unit in codeUnits)
			{
				unit.Compile(compiler);
			}
		}

		public static ValueContext GetPopulatedContext(params IRepository[] repositories)
		{
			var context = new ValueContext();

			// Load predefined variables from repositories
			foreach (IRepository repository in repositories)
			{
				var source = new TokenSource("", "_" + repository.GetType().Name, -1, 1);

				void RegisterVariable(string name, Type type, bool isReadOnly, bool isStaticType)
				{
					source.code = name;
					var token = new IdentifierToken(source) { IsParsed = true };
					var value = new Variable(name, token, type, isReadOnly, isStaticType);
					context.RegisterValueGlobally(value);
				}

				foreach (var (name, type) in repository.RegisterVariables())
					RegisterVariable(name, type, false, false);

				foreach (var (name, type) in repository.RegisterReadOnlyVariables())
					RegisterVariable(name, type, true, false);

				foreach (var (name, type) in repository.RegisterStaticTypes())
					RegisterVariable(name, type, true, true);
			}

			return context;
		}

		public string AssembleIntoString()
		{
			return AssembleUnitsIntoString(codeUnits) ?? string.Empty;
		}

		public static string AssembleUnitsIntoString(List<CodeUnit> codeUnits)
		{
			if (codeUnits == null)
				return null;
			if (codeUnits.Count == 0)
				return null;

			// Assemble into code rows
			var rows = new RowBuilder();

			foreach (CodeUnit unit in codeUnits)
			{
				rows.AppendLine(unit.AssembleIntoString());
			}

			return rows.ToString();
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