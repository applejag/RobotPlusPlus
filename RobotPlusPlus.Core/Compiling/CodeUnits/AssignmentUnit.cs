using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Structures.G1ANT;
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

			if (LHSExpression.Token is PunctuatorToken)
			{
				RHSExpression.NeedsCSSnippet =
				LHSExpression.NeedsCSSnippet = true;
			}
            else if (RHSExpression.GetAsG1ANTCommandUnit() != null)
		    {
                // Add LHS as result value
		        CommandUnit cmd = RHSExpression.GetAsG1ANTCommandUnit();
		        G1ANTParameterInfo[] parameters = ((G1ANTMethodInfo)cmd.MethodInfo).GetG1ANTParameters();
		        G1ANTParameterInfo[] matches = parameters
		            .Where(p => p.ParameterRawType == typeof(Variable))
		            .ToArray();

		        if (matches.Length == 0)
		            throw new CompileParameterNamedDoesntExistException(cmd.MethodInfo, "result", cmd.Token);
                if (matches.Length > 1)
                    throw new CompileParameterDuplicateException(cmd.MethodInfo, matches[0], cmd.Token);

		        ParameterInfo param = matches[0];
		        cmd.Arguments.Add(new CommandUnit.NamedArgument(param.Position, param.Name, LHSExpression.Token, LHSExpression));
		    }
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			foreach (CodeUnit pre in RHSExpression.PreUnits)
			{
				rows.AppendLine(pre.AssembleIntoString());
			}

		    if (RHSExpression.GetAsG1ANTCommandUnit() != null)
		    {
		        CommandUnit cmd = RHSExpression.GetAsG1ANTCommandUnit();
		        rows.AppendLine(cmd.AssembleMethodIntoString());
		    }
		    else if (LHSExpression.Token is PunctuatorToken dot)
			{
				if (!(LHSExpression.ContainerType is CSharpType cs))
					throw new CompileUnexpectedTokenException(LHSExpression.Token);

				string container = LHSExpression.StringifyToken(LHSExpression.ContainerToken);
				string containerType = StringifyTypeFullName(cs.Type);
				string property = LHSExpression.StringifyToken(LHSExpression.Token).Substring(container.Length);
				string expression = RHSExpression.StringifyToken(RHSExpression.Token);
				rows.AppendLine("{0}=⊂new Func<{3}, {3}>(({3} _)=>{{_{1}={2};return _;}})({0})⊃", 
				    container,  // ♥myRect
                    property, // .Width
				    expression, // 50
				    containerType); // System.Drawing.Rectangle
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