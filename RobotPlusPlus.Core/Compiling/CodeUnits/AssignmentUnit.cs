using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class AssignmentUnit : CodeUnit
	{
		public ExpressionUnit Expression { get; }
		public string VariableOriginalName { get; }
		public string VariableGeneratedName { get; private set; }

		public AssignmentUnit([NotNull] OperatorToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			if (token.OperatorType != OperatorToken.Type.Assignment)
				throw new CompileUnexpectedTokenException(token);
			
			if (!(token.LHS is IdentifierToken id))
				throw new CompileUnexpectedTokenException(token);

			Expression = new ExpressionUnit(token.RHS, this);
			VariableOriginalName = id.SourceCode;
		}

		public override void PreCompile(Compiler compiler)
		{
			Expression.PreCompile(compiler);
		}

		public override void PostCompile(Compiler compiler)
		{
			Expression.PostCompile(compiler);
		}

		public override void Compile(Compiler compiler)
		{
			Expression.Compile(compiler);

			// Register variable, or use already registered
			VariableGeneratedName = compiler.VariableContext.GetOrGenerateName(VariableOriginalName);
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			foreach (CodeUnit pre in Expression.PreUnits)
			{
				rows.AppendLine(pre.AssembleIntoString());
			}

			rows.AppendLine("♥{0}={1}", VariableGeneratedName, Expression.AssembleIntoString());

			foreach (CodeUnit post in Expression.PostUnits)
			{
				rows.AppendLine(post.AssembleIntoString());
			}

			return rows.ToString();
		}
	}
}