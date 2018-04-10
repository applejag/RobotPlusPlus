using System.Collections.Generic;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class AssignmentUnit : CodeUnit
	{
		public ExpressionUnit Expression { get; }
		public IdentifierToken VariableOriginalToken { get; }
		public string VariableGeneratedName { get; private set; }

		public AssignmentUnit([NotNull] OperatorToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			if (token.OperatorType != OperatorToken.Type.Assignment)
				throw new CompileUnexpectedTokenException(token);
			
			if (!(token.LHS is IdentifierToken id))
				throw new CompileUnexpectedTokenException(token);

			Expression = new ExpressionUnit(token.RHS, this);
			VariableOriginalToken = id;
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

		public static (ExpressionUnit, IdentifierTempToken) CreateTemporaryExpression([NotNull] ExpressionUnit expression)
		{
			(CodeUnit preUnit, IdentifierTempToken id) = CreateTemporaryAssignment(expression.Token, expression.Parent);

			var exp = new ExpressionUnit(id, expression.Parent);

			// Old preunits
			foreach (CodeUnit pre in expression.PreUnits)
				exp.PreUnits.Add(pre);
			// Temp assignment
			exp.PreUnits.Add(preUnit);
			// Old postunits
			foreach (CodeUnit post in expression.PostUnits)
				exp.PreUnits.Add(post);

			return (exp, id);
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
			VariableGeneratedName = compiler.Context.GetOrRegisterName(VariableOriginalToken);
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