using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Tokenizing;

namespace RobotPlusPlus.Asserting.Definitions
{
	public abstract class CodeUnit
	{
		[NotNull]
		private readonly List<CodeUnit> units;

		public IReadOnlyList<CodeUnit> Units => units;
		public CodeUnit FirstToken => units.Count > 0 ? units[0] : null;
		public CodeUnit LastToken => units.Count > 0 ? units[units.Count - 1] : null;

		protected CodeUnit(IEnumerable<CodeUnit> units)
		{
			this.units = units?.ToList() ?? new List<CodeUnit>();
		}

		protected CodeUnit() : this(null) { }
	}
}