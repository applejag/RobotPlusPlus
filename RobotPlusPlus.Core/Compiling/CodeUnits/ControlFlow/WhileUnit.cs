using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public class WhileUnit : AbstractFlowUnit
	{
		public string GeneratedLabelStart { get; private set; }
		public string GeneratedLabelEnd { get; private set; }

		public WhileUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{ }

		public override void Compile(Compiler compiler)
		{
			compiler.Context.PushLayer();
			
			GeneratedLabelStart = compiler.Context.RegisterTempName("while");
			GeneratedLabelEnd = compiler.Context.RegisterTempName("whileend");

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);

			compiler.Context.PopLayer();
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();
			bool isEmpty = !CodeBlock.IsEmpty;

			if (isEmpty) rows.AppendLine(AssembleJumpIfCondition(GeneratedLabelEnd, true));
			rows.AppendLine("➜{0}", GeneratedLabelStart);
			if (isEmpty) rows.AppendLine(CodeBlock.AssembleIntoString());
			rows.AppendLine(AssembleJumpIfCondition(GeneratedLabelStart));
			if (isEmpty) rows.AppendLine("➜{0}", GeneratedLabelEnd);

			return rows.ToString();
		}
	}
}