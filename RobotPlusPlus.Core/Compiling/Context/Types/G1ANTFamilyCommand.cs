using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class G1ANTFamilyCommand : G1ANTFamily
	{
		[NotNull]
		public G1ANTRepository.CommandElement Command { get; }

		public G1ANTFamilyCommand(
			[NotNull] G1ANTRepository.CommandFamilyElement family,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[NotNull] G1ANTRepository.CommandElement command)
			: base(family, globalArguments)
		{
			Command = command;
		}
	}
}