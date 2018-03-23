using System.Collections.Generic;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public abstract class CodeUnit
	{
		public List<CodeUnit> PreUnits { get; }
		public List<CodeUnit> PostUnits { get; }

		public Token Source { get; }

		protected CodeUnit([NotNull] CodeUnit parent, [NotNull] Token source)
		{
			Source = source;
			PreUnits = parent.PreUnits;
			PostUnits = parent.PostUnits;
		}

		protected CodeUnit([NotNull] Token source)
		{
			Source = source;
			PreUnits = new List<CodeUnit>();
			PostUnits = new List<CodeUnit>();
		}

	}
}