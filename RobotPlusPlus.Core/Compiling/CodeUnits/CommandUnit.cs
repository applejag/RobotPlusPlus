using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class CommandUnit : CodeUnit
	{
		public CommandUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null) : base(token, parent)
		{
		}

		public override void Compile(Compiler compiler)
		{
			throw new System.NotImplementedException();
		}

		public override string AssembleIntoString()
		{
			throw new System.NotImplementedException();
		}
	}
}