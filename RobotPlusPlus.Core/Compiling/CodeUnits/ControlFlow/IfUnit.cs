using System.Collections.Generic;
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
		public string GeneratedTemporaryVariable { get; private set; }

		public IfUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
			Condition = new ExpressionUnit(token.Condition, this);
			CodeBlock = new CodeBlockUnit(token.CodeBlock);
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
			GeneratedTemporaryVariable = Condition.PostUnits.Count > 0
				? compiler.Context.RegisterName("tmp") : null;

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			foreach (CodeUnit pre in Condition.PreUnits)
				rows.AppendLine(pre.AssembleIntoString());

			if (GeneratedTemporaryVariable == null)
			{
				rows.AppendLine("jump label ➜{0} if ⊂!({1})⊃", GeneratedLabel, Condition.AssembleIntoString());
			}
			else
			{
				rows.AppendLine("♥{0}={1}", GeneratedTemporaryVariable, Condition.AssembleIntoString());

				foreach (CodeUnit post in Condition.PostUnits)
					rows.AppendLine(post.AssembleIntoString());

				rows.AppendLine("jump label ➜{0} if ⊂!♥{1}⊃", GeneratedLabel, GeneratedTemporaryVariable);
			}

			rows.AppendLine(CodeBlock.AssembleIntoString());
			rows.AppendLine("➜{0}", GeneratedLabel);

			return rows.ToString();
		}
	}
}