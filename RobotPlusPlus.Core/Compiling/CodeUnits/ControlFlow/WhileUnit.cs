using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public class WhileUnit : AbstractFlowUnit
	{
		public Label GeneratedLabelStart { get; private set; }
		public Label GeneratedLabelEnd { get; private set; }

		public WhileUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{ }

		public override void Compile(Compiler compiler)
		{
			compiler.Context.PushLayer();
			
			GeneratedLabelStart = compiler.Context.RegisterLabelDecayed("while");
			GeneratedLabelEnd = CodeBlock.IsEmpty
				? null : compiler.Context.RegisterLabelDecayed("whileend");

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);

			ValidateCondition();

			compiler.Context.PopLayer();
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();
			bool isEmpty = !CodeBlock.IsEmpty;

			if (isEmpty) rows.AppendLine(AssembleJumpIfCondition(GeneratedLabelEnd, true));
			rows.AppendLine("➜{0}", GeneratedLabelStart.Generated);
			if (isEmpty) rows.AppendLine(CodeBlock.AssembleIntoString());
			rows.AppendLine(AssembleJumpIfCondition(GeneratedLabelStart));
			if (isEmpty) rows.AppendLine("➜{0}", GeneratedLabelEnd.Generated);

			return rows.ToString();
		}
	}
}