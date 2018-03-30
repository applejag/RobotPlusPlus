using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class CommandUnit : CodeUnit
	{
		public ExpressionUnit Identifier { get; }
		public List<ExpressionUnit> Arguments { get; }
		public Dictionary<string, ExpressionUnit> NamedArguments { get; }

		public CommandUnit([NotNull] FunctionCallToken token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
			Identifier = new ExpressionUnit(token.LHS, parent);

			List<Argument> arguments = SplitArguments(token.ParentasesGroup);

			Arguments = arguments.OfType<IndexedArgument>()
				.Select(a => a.expression).ToList();

			NamedArguments = arguments.OfType<NamedArgument>()
				.ToDictionary(a => a.name, a => a.expression);
		}

		public override void Compile(Compiler compiler)
		{
			throw new System.NotImplementedException();
		}

		public override string AssembleIntoString()
		{
			throw new System.NotImplementedException();
		}

		private List<Argument> SplitArguments(PunctuatorToken parentases)
		{
			string name = null;

			var arguments = new List<Argument>();

			var iterator = new IteratedList<Token>(parentases);
			foreach (Token token in iterator)
			{
				// Check if named or indexed
				if (token is PunctuatorToken pun
				    && pun.PunctuatorType == PunctuatorToken.Type.Colon)
				{
					if (iterator.Next == null)
						throw new ParseUnexpectedTrailingTokenException(token, null);

					if (PunctuatorToken.IsSeparatorOfChar(iterator.Next, ','))
						throw new CompileUnexpectedTokenException(iterator.Next);

					name = pun.ColonName.SourceCode;
					arguments.Add(new NamedArgument(name, new ExpressionUnit(iterator.PopNext(), this)));
				} else if (name != null)
				{
					throw new CompileException($"Expected named parameter, got <{token.SourceCode}>", token);
				}
				else
				{
					arguments.Add(new IndexedArgument(new ExpressionUnit(token, this)));
				}

				// Check for comma seperators
				if (PunctuatorToken.IsSeparatorOfChar(iterator.Next, ','))
				{
					if (iterator.Count - iterator.Index == 2)
						throw new CompileException("Unexpected separator <,> with no followup parameter.", iterator.Next);

					// Remove the comma
					iterator.PopNext();
				}
			}

			return arguments;
		}

		#region Argument classes

		private abstract class Argument
		{
			public readonly ExpressionUnit expression;

			protected Argument(ExpressionUnit expression)
			{
				this.expression = expression;
			}
		}

		private class NamedArgument : Argument
		{
			public readonly string name;

			public NamedArgument(string name, ExpressionUnit expression)
				: base(expression)
			{
				this.name = name;
			}
		}

		private class IndexedArgument : Argument
		{
			public IndexedArgument(ExpressionUnit expression)
				: base(expression)
			{ }
		}

		#endregion
	}
}