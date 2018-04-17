using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class Label : AbstractValue
	{
		public bool IsTemporary => Identifier == string.Empty;

		public Label([NotNull] string generated, [NotNull] string identifier)
			: base(generated, identifier)
		{ }

		public Label([NotNull] string generated)
			: base(generated, string.Empty)
		{ }
	}
}