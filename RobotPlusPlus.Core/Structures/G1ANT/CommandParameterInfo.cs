using System;
using System.Reflection;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Structures.G1ANT
{
	public class CommandParameterInfo : ParameterInfo
	{
		[NotNull]
		public override MemberInfo Member { get; }
		[NotNull]
		public G1ANTRepository.ArgumentElement ArgumentElement { get; }

		public override Type ParameterType => ArgumentElement.EvaluateType();
		public override bool HasDefaultValue => !ArgumentElement.Required;

		[NotNull]
		public override string Name => ArgumentElement.Name;
		public override int Position { get; }

		internal CommandParameterInfo(
			[NotNull] CommandMethodInfo parent,
			[NotNull] G1ANTRepository.ArgumentElement argument,
			int position)
		{
			Member = parent;
			ArgumentElement = argument;
			Position = position;
		}
	}
}