using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class CodeBlockUnit : CodeUnit
	{
		public List<CodeUnit> CodeUnits { get; }
		public bool IsEmpty => CodeUnits.Count == 0;

		public CodeBlockUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			if (token is PunctuatorToken pun
			    && pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases
			    && pun.Character == '{')
				// Add group
				CodeUnits = new List<CodeUnit>(token.Select(token1
					=> CompileParsedToken(token1, this)));
			else
				// Add single
				CodeUnits = new List<CodeUnit> {CompileParsedToken(token, this)};
		}

		public override void Compile(Compiler compiler)
		{
			compiler.Context.PushLayer();
			Compiler.CompileUnits(CodeUnits, compiler);
			compiler.Context.PopLayer();
		}

		public override string AssembleIntoString()
		{
			return Compiler.AssembleUnitsIntoString(CodeUnits);
		}
	}
}