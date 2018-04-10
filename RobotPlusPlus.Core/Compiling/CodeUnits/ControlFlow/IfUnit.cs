using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow
{
	public class IfUnit : CodeUnit
	{
		public ExpressionUnit Condition { get; }
		public CodeBlockUnit CodeBlock { get; }
		public CodeBlockUnit ElseBlock { get; }
		public string GeneratedLabelEnd { get; private set; }
		public string GeneratedLabelElse { get; private set; }

		public IfUnit([NotNull] StatementToken token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
			Condition = new ExpressionUnit(token.Condition, this);
			CodeBlock = new CodeBlockUnit(token.CodeBlock, this);
			ElseBlock = token.ElseBlock == null
				? null : new CodeBlockUnit(token.ElseBlock, this);

			if (Condition.PostUnits.Count > 0)
			{
				(Condition, _) = Condition.ExtractIntoTempAssignment();
			}
		}

		public override void Compile(Compiler compiler)
		{
			compiler.Context.PushLayer();

			GeneratedLabelEnd = compiler.Context.RegisterTempName("ifend");
			GeneratedLabelElse = ElseBlock == null
				? null : compiler.Context.RegisterTempName("ifelse");

			Condition.Compile(compiler);
			CodeBlock.Compile(compiler);
			ElseBlock?.Compile(compiler);

			compiler.Context.PopLayer();
		}

		public override string AssembleIntoString()
		{
			var rows = new RowBuilder();

			// Condition pre units
			foreach (CodeUnit pre in Condition.PreUnits)
				rows.AppendLine(pre.AssembleIntoString());

			if (Condition.PostUnits.Count > 0)
				throw new CompileUnexpectedTokenException(Condition.PostUnits[0].Token);
			
			// Echo condition
			string label = ElseBlock == null ? GeneratedLabelEnd : GeneratedLabelElse;
			if (Condition.Token is IdentifierToken || Condition.Token is LiteralToken)
				rows.AppendLine("jump label ➜{0} if ⊂!{1}⊃", label, Condition.AssembleIntoString());
			else
				rows.AppendLine("jump label ➜{0} if ⊂!({1})⊃", label, Condition.AssembleIntoString());

			// Echo code block
			rows.AppendLine(CodeBlock.AssembleIntoString());

			// Echo else code block
			if (ElseBlock != null)
			{
				rows.AppendLine("jump label ➜{0}", GeneratedLabelEnd);
				rows.AppendLine("➜{0}", GeneratedLabelElse);
				rows.AppendLine(ElseBlock.AssembleIntoString());
			}

			// Echo end label
			rows.AppendLine("➜{0}", GeneratedLabelEnd);

			return rows.ToString();
		}
	}
}