using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Structures.G1ANT
{
	public class G1ANTMethodInfo : MethodInfo
	{
		[NotNull]
		public G1ANTRepository.CommandElement Command { get; }
		[CanBeNull]
		public G1ANTRepository.CommandFamilyElement CommandFamily { get; }
		[NotNull]
		public override Type ReflectedType => CommandFamily?.GetType();

		public override Type DeclaringType => CommandFamily?.GetType();
		public override string Name => Command.Name;
		public G1ANTParameterInfo[] CommandArguments { get; }
		public G1ANTParameterInfo[] GlobalArguments { get; }

		public override ParameterInfo[] GetParameters()
		{
			return CommandArguments
				.Concat(GlobalArguments)
				.Cast<ParameterInfo>()
				.ToArray();
		}

		internal G1ANTMethodInfo(
			[NotNull] G1ANTRepository.CommandElement command,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[CanBeNull] G1ANTRepository.CommandFamilyElement family = null)
		{
			Command = command;
			CommandFamily = family;

			CommandArguments = command.Arguments
				.Select((a, i) => new G1ANTParameterInfo(this, a, i)).ToArray();
			GlobalArguments = globalArguments.Arguments
				.Select((a, i) => new G1ANTParameterInfo(this, a, i + CommandArguments.Length)).ToArray();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			throw new NotImplementedException();
		}

		public override MethodAttributes Attributes { get; } = MethodAttributes.Public | MethodAttributes.Static;
		public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();
	}
}