using System;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class G1ANTCommand : AbstractValue
	{
		[NotNull]
		public G1ANTRepository.CommandElement Command { get; }
		[NotNull]
		public G1ANTRepository.GlobalArgumentsElement GlobalArguments { get; }
		[CanBeNull]
		public G1ANTFamily Family { get; }

		public G1ANTCommand(
			[NotNull] G1ANTRepository.CommandElement command,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[CanBeNull] G1ANTFamily family = null)
			: base(command.Name, command.Name)
		{
			Command = command;
			GlobalArguments = globalArguments;
			Family = family;
		}

		public G1ANTMethodInfo GetMethod(params Type[] types)
		{
			return G1ANTMethodInfo.GetMethod(Command, GlobalArguments, Family?.Family, types);
		}
	}
}