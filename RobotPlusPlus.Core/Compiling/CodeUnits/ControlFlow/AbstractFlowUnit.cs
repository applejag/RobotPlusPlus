using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public abstract class AbstractFlowUnit : CodeUnit
	{
		public ExpressionUnit Condition { get; }
		public CodeBlockUnit CodeBlock { get; }

		protected AbstractFlowUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
			CodeBlock = new CodeBlockUnit(token.CodeBlock, this);
			Condition = new ExpressionUnit(token.Condition, this);

			if (Condition.PostUnits.Count > 0)
			{
				(Condition, _) = Condition.ExtractIntoTempAssignment();
			}
		}

		protected string AssembleJumpIfCondition(Label label, bool inverted = false)
		{
			var rows = new RowBuilder();

			// Condition pre units
			foreach (CodeUnit pre in Condition.PreUnits)
				rows.AppendLine(pre.AssembleIntoString());

			if (Condition.PostUnits.Count > 0)
				throw new CompileUnexpectedTokenException(Condition.PostUnits[0].Token);

			string condition;
			if (inverted && !(Condition.Token is IdentifierToken || Condition.Token is LiteralToken))
				condition = $"({Condition.AssembleIntoString()})";
			else
				condition = Condition.AssembleIntoString();


			rows.AppendLine("jump label ➜{0} if ⊂{1}{2}⊃", label.Generated, inverted ? "!" : "", condition);
			return rows.ToString();
		}

		protected void ValidateCondition()
		{
			// Validate condition
			if (!TypeChecking.CanImplicitlyConvert(Condition.OutputType, typeof(bool)))
				throw new CompileTypeConvertImplicitException(Condition.Token, typeof(bool), Condition.OutputType);
		}
	}
}