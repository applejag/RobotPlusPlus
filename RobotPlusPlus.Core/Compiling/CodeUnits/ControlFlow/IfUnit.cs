using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public class IfUnit : AbstractFlowUnit
	{
		public CodeBlockUnit ElseBlock { get; }
		public string GeneratedLabelEnd { get; private set; }
		public string GeneratedLabelElse { get; private set; }

		public IfUnit FirstParentIf { get; private set; }

		public IfUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			ElseBlock = token.ElseBlock == null
				? null : new CodeBlockUnit(token.ElseBlock, this);
		}

		public override void Compile(Compiler compiler)
		{
			compiler.Context.PushLayer();

			FirstParentIf = ParentSearchWhereImLast(Parent);

			GeneratedLabelElse = ElseBlock?.IsEmpty != false || CodeBlock.IsEmpty
				? null : compiler.Context.RegisterTempName("ifelse");

			GeneratedLabelEnd = FirstParentIf?.GeneratedLabelEnd
								?? compiler.Context.RegisterTempName("ifend");

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);
			ElseBlock?.Compile(compiler);

			compiler.Context.PopLayer();
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			// Echo condition
			string label = GeneratedLabelElse ?? GeneratedLabelEnd;
			rows.AppendLine(AssembleJumpIfCondition(label, !CodeBlock.IsEmpty));

			// Echo code block
			rows.AppendLine(CodeBlock.AssembleIntoString());

			// Echo else code block
			if (ElseBlock?.IsEmpty == false)
			{
				if (!CodeBlock.IsEmpty)
				{
					rows.AppendLine("jump label ➜{0}", GeneratedLabelEnd);
					rows.AppendLine("➜{0}", GeneratedLabelElse);
				}

				rows.AppendLine(ElseBlock.AssembleIntoString());
			}

			// Echo end label
			if (FirstParentIf == null)
				rows.AppendLine("➜{0}", GeneratedLabelEnd);

			return rows.ToString();
		}

		[CanBeNull]
		private IfUnit ParentSearchWhereImLast([CanBeNull] CodeUnit parent)
		{
			CodeUnit child = this;

			while (true)
			{
				switch (parent)
				{
					// Bedrock
					case null:
						return null;

					// Not last?
					case CodeBlockUnit block when block.CodeUnits.Last() != child:
						return null;

					// THIS IS IT
					case IfUnit unit:
						if (unit.ElseBlock != child)
							return null;
						return unit;
				}

				child = parent;
				parent = parent.Parent;
			}
		}
	}
}