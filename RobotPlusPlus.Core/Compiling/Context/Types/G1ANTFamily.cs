using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class G1ANTFamily : AbstractValue
	{
		[NotNull]
		public G1ANTRepository.CommandFamilyElement Family { get; }

		[NotNull, ItemNotNull]
		public G1ANTCommand[] Commands { get; }

		public G1ANTFamily(
			[NotNull] G1ANTRepository.CommandFamilyElement family,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments)
			: base(family.Name, family.Name)
		{
			Family = family;

			Commands = family.Commands
				.Select(c => new G1ANTCommand(c, globalArguments, this))
				.ToArray();
		}
	}
}