using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class G1ANTCommand : AbstractValue, IMethod
	{
		[NotNull]
		public G1ANTRepository.CommandElement Command { get; }
		[NotNull]
		public G1ANTRepository.GlobalArgumentsElement GlobalArguments { get; }
		[CanBeNull]
		public G1ANTFamily Family { get; }

		public MethodInfo[] MethodInfos { get; }

		public G1ANTCommand(
			[NotNull] G1ANTRepository.CommandElement command,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[CanBeNull] G1ANTFamily family = null)
			: base(command.Name, command.Name)
		{
			Command = command;
			GlobalArguments = globalArguments;
			Family = family;

			MethodInfos = G1ANTMethodInfo.ListMethods(command, globalArguments, family?.Family)
				.Cast<MethodInfo>().ToArray();
		}
	}
}