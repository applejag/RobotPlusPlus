using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class CodeBlockUnit : CodeUnit
	{
		public List<CodeUnit> codeUnits;

		public CodeBlockUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
			if (token is PunctuatorToken pun
			    && pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases
			    && pun.Character == '{')
				// Add group
				codeUnits = new List<CodeUnit>(token.Select(CompileParsedToken));
			else
				// Add single
				codeUnits = new List<CodeUnit> {CompileParsedToken(token)};
		}

		public override void PreCompile(Compiler compiler)
		{
			compiler.Context.PushLayer();
		}

		public override void PostCompile(Compiler compiler)
		{
			compiler.Context.PopLayer();
		}

		public override void Compile(Compiler compiler)
		{
			Compiler.CompileUnits(codeUnits, compiler);
		}

		public override string AssembleIntoString()
		{
			return Compiler.AssembleUnitsIntoString(codeUnits);
		}
	}
}