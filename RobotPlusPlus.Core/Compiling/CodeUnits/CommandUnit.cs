using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context.Types;
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

		public ExpressionUnit Method { get; }

		public MethodInfo MethodInfo { get; private set; }

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
				foreach ((string name, Token expr) in addedArguments)
				{
					Arguments.Add(new NamedArgument(-1, name, expr, this));
				}
			}

			Method = new ExpressionUnit(token.LHS, this);
		}

		public override void Compile(Compiler compiler)
		{
			Method.Compile(compiler);
			Exception error = null;
			MethodInfo[] methodInfos = GetMethodInfos();

			foreach (MethodInfo methodInfo in methodInfos)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();

				// Check if this works, if not check next
				try
				{

					foreach (Argument argument in Arguments)
					{
						// Find parameter
						ParameterInfo param;
						if (argument is NamedArgument named)
						{
							if (!parameters.TryFirst(p => p.Name == named.name, out param))
								throw new CompileParameterNamedDoesntExistException(methodInfo, named.name, named.expressionToken);
						}
						else
						{
							if (argument.index < parameters.Length)
								param = parameters[argument.index];
							else
								throw new CompileParameterIndexedDoesntExistException(methodInfo, argument.index, argument.expressionToken);
						}

						// Apply settings from parameter
						if (param is G1ANTParameterInfo g1 && g1.ArgumentElement.Type == G1ANTRepository.Structure.Variable)
						{
							argument.expression.Usage = ExpressionUnit.UsageType.Write;
							argument.expression.InputType = new CSharpType(g1.ArgumentElement.EvaluateVariableType(), g1.Name);
						}
						else
						{
							argument.expression.Usage = ExpressionUnit.UsageType.Read;
							argument.expression.InputType = null;
						}


						// Compile
						argument.expression.Compile(compiler);
					}
					
					if (EvalMethodInfo(methodInfo))
					{
						MethodInfo = methodInfo;
						return;
					}
				}
				catch (CompileFunctionException e)
				{
					error = e;
				}
			}

			// Tell us what went wrong
			if (error != null && methodInfos.Length == 1)
				throw error;

			// Didn't find it amoung multiple overloads...
			throw new CompileFunctionException($"Method <{methodInfos[0].Name}> has no overload matching the parameters!", Token, error);
		}

		[NotNull, ItemNotNull]
		private MethodInfo[] GetMethodInfos()
		{
			if (Method.OutputType is IMethod met)
				return met.MethodInfos;

			throw new CompileFunctionException($"Invalid method type, <{Method.OutputType?.GetType().Name ?? "null"}>", Token);
		}

		private bool EvalMethodInfo(MethodInfo method)
		{
			foreach (Argument argument in Arguments)
			{
				if (!(argument.expression.OutputType is CSharpType))
					throw new CompileFunctionException(
						$"Invalid token type <{argument.expression.OutputType?.GetType().Name ?? "null"}> for parameter <{(argument is NamedArgument n ? n.name : argument.index.ToString())}>.",
						argument.expressionToken);
			}

			return G1ANTMethodInfo.MethodMatches((FunctionCallToken)Token, method, Arguments.ToArray());
		}

		public override string AssembleIntoString()
		{
			var row = new RowBuilder();

			// Pre units
			foreach (Argument argument in Arguments)
				foreach (CodeUnit preUnit in argument.expression.PreUnits)
					row.AppendLine(preUnit.AssembleIntoString());

			// Assemble command with arguments
			row.AppendLine(AssembleMethodIntoString());

			// Post units
			foreach (Argument argument in Arguments)
				foreach (CodeUnit postUnit in argument.expression.PostUnits)
					row.AppendLine(postUnit.AssembleIntoString());

			return row.ToString();
		}

		[Pure, NotNull]
		private string AssembleMethodIntoString()
		{
			switch (MethodInfo)
			{
				case G1ANTMethodInfo g1:
					// G1ANT command assembly
					var parts = new List<string>
					{
						g1.CommandFamily != null
							? $"{g1.CommandFamily.Name}.{g1.Name}"
							: g1.Name
					};

					ParameterInfo[] parameters = g1.GetParameters();
					foreach (Argument argument in Arguments)
					{
						string name = parameters.First(p => p.Position == argument.index).Name;
						parts.Add($"{name} {argument.expression.AssembleIntoString()}");
					}

					return string.Join(' ', parts);

				case MethodInfo cs:
					return "";

				default:
					throw new InvalidOperationException($"Unknown method type <{Method.OutputType?.GetType().FullName ?? "null"}>.");
			}
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

					args.Add(new NamedArgument(-1, pun.ColonName, iterator.PopNext(), this));
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
			public int index;

			public ExpressionUnit expression;

			public Argument(int index, Token expressionToken, CodeUnit parent)
			{
				this.expressionToken = expressionToken;
				expression = new ExpressionUnit(expressionToken, parent);
				this.index = index;
			}

			public override string ToString()
			{
				string output = (expression?.OutputType as CSharpType)?.Type?.FullName ?? "null";
				return $"[{index}] {output} {expressionToken}";
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

			public override string ToString()
			{
				string output = (expression?.OutputType as CSharpType)?.Type?.FullName ?? "null";
				return index == -1
					? $@"[""{name}""] {output} {expressionToken}"
					: $@"[{index}, ""{name}""] {output} {expressionToken}";
			}
		}

		#endregion
	}
}