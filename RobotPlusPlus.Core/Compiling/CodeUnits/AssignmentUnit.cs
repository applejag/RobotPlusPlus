using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class AssignmentUnit : CodeUnit
	{
		public ExpressionUnit RHSExpression { get; }
		public ExpressionUnit LHSExpression { get; }

		public AssignmentUnit([NotNull] OperatorToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			if (token.OperatorType != OperatorToken.Type.Assignment)
				throw new CompileUnexpectedTokenException(token);

			RHSExpression = new ExpressionUnit(token.RHS, this);
			LHSExpression = new ExpressionUnit(token.LHS, this, ExpressionUnit.UsageType.Write);
		}

		public static (CodeUnit, IdentifierTempToken) CreateTemporaryAssignment([NotNull] Token RHS, [CanBeNull] CodeUnit parent = null)
		{
			// Fabricate tokens
			TokenSource source = RHS.source;
			source.code = "=";
			var op = new OperatorToken(source);

			source.code = string.Empty;
			var id = new IdentifierTempToken(source);

			// Fabricate environment
			var env = new IteratedList<Token>(new List<Token>
			{
				id, op, RHS
			});

			// Parse
			env.ParseTokenAt(1);

			// Fabricate assignment unit
			CodeUnit tempUnit = CompileParsedToken(op, parent);

			return (tempUnit, id);
		}

		public override void Compile(Compiler compiler)
		{
			RHSExpression.Compile(compiler);

			LHSExpression.InputType = RHSExpression.OutputType;
			LHSExpression.Compile(compiler);

			if (!TypeChecking.CanImplicitlyConvert(RHSExpression.OutputType, LHSExpression.OutputType))
				throw new CompileTypeConvertImplicitAssignmentException(LHSExpression.Token, RHSExpression.OutputType, LHSExpression.OutputType);

			if (LHSExpression.Token is PunctuatorToken)
			{
				RHSExpression.NeedsCSSnippet =
				LHSExpression.NeedsCSSnippet = true;
			}
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			foreach (CodeUnit pre in RHSExpression.PreUnits)
			{
				rows.AppendLine(pre.AssembleIntoString());
			}

			if (LHSExpression.Token is PunctuatorToken dot)
			{
				string container = LHSExpression.StringifyToken(LHSExpression.ContainerToken);
				string containerType = StringifyTypeFullName(LHSExpression.ContainerType);
				string property = LHSExpression.StringifyToken(LHSExpression.Token).Substring(container.Length);
				string expression = RHSExpression.StringifyToken(RHSExpression.Token);
				rows.AppendLine("{0}=⊂new Func<{3}>(()=>{{var _={0};_{1}={2};return _;}})()⊃", container, property, expression, containerType);
			} else
				rows.AppendLine("{0}={1}", LHSExpression.AssembleIntoString(), RHSExpression.AssembleIntoString());

			foreach (CodeUnit post in RHSExpression.PostUnits)
			{
				rows.AppendLine(post.AssembleIntoString());
			}

			return rows.ToString();
		}

		private static string StringifyTypeFullName(Type type)
		{
			return type.ContainsGenericParameters
				? $"{type.Namespace}.{type.Name}<{string.Join(",", type.GenericTypeArguments.Select(StringifyTypeFullName))}>"
				: type.FullName;
		}
	}
}