using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public class IfUnit : CodeUnit
	{
		public ExpressionUnit Condition { get; }
		public CodeBlockUnit CodeBlock { get; }
		public string GeneratedLabel { get; private set; }

		public IfUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
			Condition = new ExpressionUnit(token.Condition, this);
			CodeBlock = new CodeBlockUnit(token.CodeBlock);

			if (Condition.PostUnits.Count > 0)
			{
				(Condition, _) = Condition.ExtractIntoTempAssignment();
			}
		}

		public override void PreCompile(Compiler compiler)
		{
			compiler.Context.PushLayer();
			Condition.PreCompile(compiler);
			CodeBlock.PreCompile(compiler);
		}

		public override void PostCompile(Compiler compiler)
		{
			Condition.PostCompile(compiler);
			CodeBlock.PostCompile(compiler);
			compiler.Context.PopLayer();
		}

		public override void Compile(Compiler compiler)
		{
			GeneratedLabel = compiler.Context.RegisterName("ifend");

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			foreach (CodeUnit pre in Condition.PreUnits)
				rows.AppendLine(pre.AssembleIntoString());

			if (Condition.Token is IdentifierToken
			    || Condition.Token is LiteralToken)
			{
				rows.AppendLine("jump label ➜{0} if ⊂!{1}⊃", GeneratedLabel, Condition.AssembleIntoString());
			}
			else
			{
				rows.AppendLine("jump label ➜{0} if ⊂!({1})⊃", GeneratedLabel, Condition.AssembleIntoString());
			}

			rows.AppendLine(CodeBlock.AssembleIntoString());
			rows.AppendLine("➜{0}", GeneratedLabel);

			return rows.ToString();
		}
	}
}