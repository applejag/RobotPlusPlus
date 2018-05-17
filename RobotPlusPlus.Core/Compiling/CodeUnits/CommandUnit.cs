﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class CommandUnit : CodeUnit
	{
		public List<Argument> Arguments { get; }

		public ExpressionUnit Container { get; }
		public string CommandName { get; }
		public string CommandFullName { get; }
		public string CommandFamilyName { get; }

		public MethodInfo Command { get; private set; }

		public CommandUnit([NotNull] FunctionCallToken token, [CanBeNull] CodeUnit parent = null,
			[CanBeNull] params (string, Token)[] addedArguments) : base(token, parent)
		{
			if (token.ParentasesGroup is null)
				throw new CompileException("Function parentases token is null.", token);
			if (token.LHS is null)
				throw new CompileException("Function callee token is null.", token);

			// Get arguments from parentases group
			Arguments = SplitArguments(token.ParentasesGroup);

			// Add result argument
			if (addedArguments != null)
			{
				foreach (var (name, expr) in addedArguments)
				{
					Arguments.Add(new NamedArgument(Arguments.Count, name, expr, this));
				}
			}

			CommandFullName = token.LHS.ToString();

			if (token.LHS is PunctuatorToken dot && dot.PunctuatorType == PunctuatorToken.Type.Dot)
			{
				Container = new ExpressionUnit(dot.DotLHS, this);
				CommandFamilyName = dot.DotLHS.ToString();
				CommandName = dot.DotRHS.ToString();
			}
			else
			{
				Container = new ExpressionUnit(token.LHS, this);
				CommandFamilyName = null;
				CommandName = token.LHS.SourceCode;
			}
		}


		public override void Compile(Compiler compiler)
		{
			Container.Compile(compiler);

			foreach (Argument argument in Arguments)
				argument.expression.Compile(compiler);

			FindCommandMethodInfo(compiler);
		}

		private void FindCommandMethodInfo(Compiler compiler)
		{
			Type[] parameters = Arguments.Select(a => a.expression.OutputType).ToArray();

			Command =
				compiler.G1ANTRepository.LookupMethodInfo(CommandFamilyName, CommandName, parameters)
				?? compiler.CSharpRepository.LookupMethodInfo(CommandFamilyName, CommandName, parameters)
				?? throw new CompileFunctionException($"Command `{CommandFullName}` does not exist (or has invalid parameters)!", Token);
		}

		public override string AssembleIntoString()
		{
			var row = new RowBuilder();

			// Pre units
			foreach (Argument argument in Arguments)
				foreach (CodeUnit preUnit in argument.expression.PreUnits)
					row.AppendLine(preUnit.AssembleIntoString());

			// Assemble command with arguments
			// TODO: Arguments
			row.AppendLine(CommandFullName);

			// Post units
			foreach (Argument argument in Arguments)
				foreach (CodeUnit postUnit in argument.expression.PostUnits)
					row.AppendLine(postUnit.AssembleIntoString());

			return row.ToString();
		}

		private List<Argument> SplitArguments(PunctuatorToken parentases)
		{
			var named = false;
			var args = new List<Argument>();

			var iterator = new IteratedList<Token>(parentases);
			foreach (Token token in iterator)
			{
				// Check if named or positional
				if (token is PunctuatorToken pun
					&& pun.PunctuatorType == PunctuatorToken.Type.Colon)
				{
					if (iterator.Next == null
						|| PunctuatorToken.IsSeparatorOfChar(iterator.Next, ','))
						throw new CompileFunctionException(
							$"Missing expression for named parameter <{pun.ColonName.SourceCode}>.", token);

					args.Add(new NamedArgument(iterator.Index, pun.ColonName, iterator.PopNext(), this));
					named = true;
				}
				else
				{
					if (named)
						throw new CompileFunctionException("Positional argument cannot precede named argument.", token);

					args.Add(new Argument(iterator.Index, token, this));
				}

				// Check for comma seperators
				if (PunctuatorToken.IsSeparatorOfChar(iterator.Next, ','))
				{
					if (iterator.Count - iterator.Index == 2)
						throw new CompileFunctionException("Unexpected separator <,> with no followup parameter.", iterator.Next);

					// Remove the comma
					iterator.PopNext();
				}
			}

			return args;
		}

		#region Argument classes

		public class Argument
		{
			public readonly Token expressionToken;
			public readonly int index;

			public ExpressionUnit expression;

			public Argument(int index, Token expressionToken, CodeUnit parent)
			{
				this.expressionToken = expressionToken;
				expression = new ExpressionUnit(expressionToken, parent);
				this.index = index;
			}
		}

		public class NamedArgument : Argument
		{
			public readonly string name;

			public NamedArgument(int index, Token nameToken, Token expressionToken, CodeUnit parent)
				: this(index, nameToken.SourceCode, expressionToken, parent)
			{ }

			public NamedArgument(int index, string name, Token expressionToken, CodeUnit parent)
				: base(index, expressionToken, parent)
			{
				this.name = name;
			}
		}

		#endregion
	}
}