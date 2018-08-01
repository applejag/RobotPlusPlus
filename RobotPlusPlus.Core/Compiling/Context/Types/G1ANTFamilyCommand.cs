using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class G1ANTFamilyCommand : G1ANTFamily, IMethod
	{
		[NotNull]
		public G1ANTRepository.CommandElement Command { get; }

		public MethodInfo[] MethodInfos { get; }

		public G1ANTFamilyCommand(
			[NotNull] G1ANTRepository.CommandFamilyElement family,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[NotNull] G1ANTRepository.CommandElement command)
			: base(family, globalArguments)
		{
			Command = command;

			MethodInfos = G1ANTMethodInfo.ListMethods(command, globalArguments, family)
				.Cast<MethodInfo>().ToArray();
		}

	}
}