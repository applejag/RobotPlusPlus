using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public class DoWhileUnit : AbstractFlowUnit
	{
		public Label GeneratedLabelStart { get; private set; }

		public DoWhileUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{ }

		public override void Compile(Compiler compiler)
		{
			compiler.Context.PushLayer();

			GeneratedLabelStart = compiler.Context.RegisterLabelDecayed("dowhile");

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);

			compiler.Context.PopLayer();
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();
			
			rows.AppendLine("➜{0}", GeneratedLabelStart.Generated);
			rows.AppendLine(CodeBlock.AssembleIntoString());
			rows.AppendLine(AssembleJumpIfCondition(GeneratedLabelStart));

			return rows.ToString();
		}
	}
}