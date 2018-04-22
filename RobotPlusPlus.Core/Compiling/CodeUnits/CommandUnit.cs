using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly List<PositionalArgument> positionalArguments;
		public List<NamedArgument> Arguments { get; }

		public string CommandName { get; }
		public string CommandFamilyName { get; }

		public G1ANTRepository.CommandFamilyElement CommandFamilyElement { get; private set; }
		public G1ANTRepository.CommandElement CommandElement { get; private set; }
		public List<G1ANTRepository.ArgumentElement> CommandArgumentElements { get; private set; }

		public CommandUnit([NotNull] FunctionCallToken token, [CanBeNull] CodeUnit parent = null,
			[CanBeNull] params (string, Token)[] addedArguments) : base(token, parent)
		{

			if (token.ParentasesGroup is null)
				throw new CompileException("Function parentases token is null.", token);
			if (token.LHS is null)
				throw new CompileException("Function callee token is null.", token);

			// Get arguments from parentases group
			(Arguments, positionalArguments) = SplitArguments(token.ParentasesGroup);

			// Add result argument
			if (addedArguments != null)
			{
				foreach (var (name, expr) in addedArguments)
				{
					Arguments.Add(new NamedArgument(name, expr));
				}
			}

			// Get command and family from LHS
			switch (token.LHS)
			{
				case IdentifierToken id:
					CommandName = id.SourceCode;
					CommandFamilyName = null;
					break;

				case PunctuatorToken dot when dot.PunctuatorType == PunctuatorToken.Type.Dot:
					if (dot.TryFirstRecursive(t => !(t is IdentifierToken), out Token faulty))
						throw new CompileException($"Dot operation for non-identifier token <{faulty?.SourceCode.EscapeString() ?? "null"}> are not supported!", faulty);

					if (PunctuatorToken.IsPunctuatorOfType(dot.DotLHS, PunctuatorToken.Type.Dot))
						throw new CompileException("Multidepth command family names are not supported!", dot.DotLHS);

					CommandFamilyName = dot.DotLHS.SourceCode;
					CommandName = dot.DotRHS.SourceCode;
					break;

				default:
					throw new CompileUnexpectedTokenException(token.LHS);
			}
		}

		public override void Compile(Compiler compiler)
		{
			CollectCommandFromRepo(compiler);

			// Alter & validate arguments
			ConvertArgumentsToNamed();
			ValidateArgumentCount();
			RegisterExpressionsToArguments();

			foreach (NamedArgument argument in Arguments)
				argument.expression.Compile(compiler);

			ValidateArgumentInputTypes();
		}

		private void CollectCommandFromRepo(Compiler compiler)
		{
			// Fetch elements from repo
			CommandFamilyElement = CommandFamilyName == null
				? null
				: compiler.G1ANTRepository.FindCommandFamily(CommandFamilyName)
				  ?? throw new CompileFunctionException($"Command family <{CommandFamilyName}> does not exist!", Token);

			CommandElement = CommandFamilyName == null
				? compiler.G1ANTRepository.FindCommand(CommandName)
				  ?? throw new CompileFunctionException($"Command <{CommandName}> does not exist!", Token)
				: compiler.G1ANTRepository.FindCommand(CommandName, CommandFamilyName)
				  ?? throw new CompileFunctionException($"Command <{CommandName}> in family <{CommandFamilyName}> does not exist!", Token);

			CommandArgumentElements = compiler.G1ANTRepository.ListCommandArguments(CommandElement);
		}

		private void ConvertArgumentsToNamed()
		{
			foreach (PositionalArgument pos in positionalArguments)
			{
				G1ANTRepository.ArgumentElement arg = CommandArgumentElements.TryGet(pos.index)
					?? throw new CompileFunctionException(
						$"Command <{CommandName}> can at max take {CommandArgumentElements.Count} arguments!",
						Token);

				Arguments.Add(new NamedArgument(arg.Name, pos.expressionToken));
			}
			positionalArguments.Clear();

			// Sort by argument index
			Dictionary<NamedArgument, int> lookup = Arguments
				.ToDictionary(a => a, a => CommandArgumentElements.FindIndex(elem => elem.Name == a.name));
			
			Arguments.Sort((a,b) => lookup[a].CompareTo(lookup[b]));
		}

		private void ValidateArgumentCount()
		{
			// Validate duplicate arguments
			NamedArgument duplicateArg = Arguments
				.GroupBy(a => a.name)
				.FirstOrDefault(g => g.Count() > 1)?.First();

			if (duplicateArg != null)
			{
				throw new CompileFunctionException($"Argument <{duplicateArg.name}> cannot be inferred multiple times!", duplicateArg.expressionToken);
			}

			// Validate arguments existance
			NamedArgument nonExistingArgument = Arguments
				.FirstOrDefault(named => CommandArgumentElements.All(argElem => argElem.Name != named.name));

			if (nonExistingArgument != null)
				throw new CompileFunctionException($"Command <{CommandName}> does not have a parameter named <{nonExistingArgument.name}>.", Token);

			// Validate all required ones
			var missingReqArgs = new List<string>();

			foreach (List<G1ANTRepository.ArgumentElement> group in CommandElement.RequiredArguments)
			{
				// Check for required arguments groups
				int matches = Arguments.Count(a => group.Any(g => g.Name == a.name));

				string displayName = group.Count == 1
					? $"<{group[0].Name}>"
					: string.Join(" or ", group.Select(a => $"<{a.Name}>"));

				if (matches == 0)
					missingReqArgs.Add(displayName);
				else if (matches > 1)
					throw new CompileFunctionException($"Command <{CommandName}> requires single value amoung arguments {displayName}.", Token);
			}

			if (missingReqArgs.Count == 1)
				throw new CompileFunctionException($"Command <{CommandName}> requires argument {missingReqArgs[0]}.", Token);
			if (missingReqArgs.Count > 1)
				throw new CompileFunctionException($"Command <{CommandName}> requires arguments {string.Join(", ", missingReqArgs)}.", Token);
		}

		private void RegisterExpressionsToArguments()
		{
			// Validate types
			foreach (var (named, argElem) in GetArgumentEnumerable())
			{
				// Only validate variable types
				switch (argElem.Type)
				{
					case G1ANTRepository.Structure.Variable:
						if (!(named.expressionToken is IdentifierToken))
							throw new CompileFunctionException(
								$"Argument <{named.name}> for command <{CommandName}> must be of type variable.", named.expressionToken);

						Type varType = argElem.EvaluateVariableType();

						named.expression = new ExpressionUnit(named.expressionToken, this, ExpressionUnit.UsageType.Write)
						{
							InputType = varType
						};
						break;

					default:
						named.expression = new ExpressionUnit(named.expressionToken, this);
						break;
				}
			}
		}

		private void ValidateArgumentInputTypes()
		{
			// Validate types
			foreach (var (named, argElem) in GetArgumentEnumerable())
			{
				// Validate variable types
				switch (argElem.Type)
				{
					case G1ANTRepository.Structure.Variable:
						break;

					case G1ANTRepository.Structure.VariableName:
						// Must be identifier
						if (!(named.expressionToken is IdentifierToken))
							throw new CompileFunctionException(
								$"Argument <{named.name}> for command <{CommandName}> must be of type variable.", named.expressionToken);
						break;

					default:
						Type expected = argElem.EvaluateType();
						Type actual = named.expression.OutputType;

						// Validate type
						if (!TypeChecking.CanImplicitlyConvert(actual, expected))
							throw new CompileTypeConvertImplicitCommandArgumentException(named, expected);
						break;
				}
			}
		}

		private IEnumerable<(NamedArgument Named, G1ANTRepository.ArgumentElement Element)> GetArgumentEnumerable()
		{
			return from arg in Arguments
				   join argElem in CommandArgumentElements on arg.name equals argElem.Name
				   select (Named: arg, Element: argElem);
		}

		public override string AssembleIntoString()
		{
			var row = new RowBuilder();

			// Pre units
			foreach (NamedArgument argument in Arguments)
				foreach (CodeUnit preUnit in argument.expression.PreUnits)
					row.AppendLine(preUnit.AssembleIntoString());

			// Assemble command with arguments
			var cmd = new StringBuilder();
			cmd.Append(CommandFamilyName == null ? CommandName : $"{CommandFamilyName}.{CommandName}");

			foreach (var (named, argElem) in GetArgumentEnumerable())
			{
				if (argElem.Type == G1ANTRepository.Structure.VariableName)
				{
					IdentifierToken variable = named.expressionToken as IdentifierToken
						?? throw new CompileUnexpectedTokenException(named.expressionToken);

					cmd.AppendFormat(" {0} ‴{1}‴", named.name, variable.GeneratedName);
				}
				else
					cmd.AppendFormat(" {0} {1}", named.name, named.expression.AssembleIntoString());
			}

			row.AppendLine(cmd.ToString());

			// Post units
			foreach (NamedArgument argument in Arguments)
				foreach (CodeUnit postUnit in argument.expression.PostUnits)
					row.AppendLine(postUnit.AssembleIntoString());

			return row.ToString();
		}

		private static (List<NamedArgument>, List<PositionalArgument>) SplitArguments(PunctuatorToken parentases)
		{
			var named = false;
			var namedArgs = new List<NamedArgument>();
			var posArgs = new List<PositionalArgument>();

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

					namedArgs.Add(new NamedArgument(pun.ColonName, iterator.PopNext()));
					named = true;
				}
				else
				{
					if (named)
						throw new CompileFunctionException("Positional argument cannot precede named argument.", token);

					posArgs.Add(new PositionalArgument(namedArgs.Count, token));
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

			return (namedArgs, posArgs);
		}

		#region Argument classes

		public abstract class Argument
		{
			public readonly Token expressionToken;

			protected Argument(Token expressionToken)
			{
				this.expressionToken = expressionToken;
			}
		}

		public class NamedArgument : Argument
		{
			public readonly string name;
			public readonly Token nameToken;
			public ExpressionUnit expression;

			public NamedArgument(Token nameToken, Token expressionToken)
				: base(expressionToken)
			{
				this.nameToken = nameToken;
				name = nameToken.SourceCode;
			}

			public NamedArgument(string name, Token expressionToken)
				: base(expressionToken)
			{
				this.name = name;
			}
		}

		public class PositionalArgument : Argument
		{
			public readonly int index;

			public PositionalArgument(int index, Token expressionToken)
				: base(expressionToken)
			{
				this.index = index;
			}
		}

		#endregion
	}
}