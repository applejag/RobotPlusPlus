using System;
using System.Collections.Generic;
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
		public override Type ReflectedType => CommandFamily?.GetType() ?? throw new InvalidOperationException();

		public override Type DeclaringType => CommandFamily?.GetType();
		public override string Name => Command.Name;
		public G1ANTParameterInfo[] CommandArguments { get; }
		public G1ANTParameterInfo[] GlobalArguments { get; }

		private G1ANTMethodInfo(
			[NotNull] G1ANTRepository.CommandElement command,
			[CanBeNull, ItemNotNull] IEnumerable<G1ANTRepository.ArgumentElement> arguments,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[CanBeNull] G1ANTRepository.CommandFamilyElement family = null)
		{
			Command = command;
			CommandFamily = family;

			CommandArguments = arguments
				?.Select((a, i) => new G1ANTParameterInfo(this, a, i)).ToArray()
				?? new G1ANTParameterInfo[0];
			GlobalArguments = globalArguments.Arguments
				.Select((a, i) => new G1ANTParameterInfo(this, a, i + CommandArguments.Length)).ToArray();
		}

		public override ParameterInfo[] GetParameters()
		{
			return CommandArguments
				.Concat(GlobalArguments)
				.Cast<ParameterInfo>()
				.ToArray();
		}

		private static G1ANTMethodInfo[] ListMethods(
			[NotNull] G1ANTRepository.CommandElement command,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[CanBeNull] G1ANTRepository.CommandFamilyElement family = null)
		{

			List<G1ANTRepository.ArgumentElement>[] overloads = command.Overloads?.Select(o => o.Arguments).ToArray()
				?? new List<G1ANTRepository.ArgumentElement>[0];

			var info = new G1ANTMethodInfo[overloads.Length + 1];
			info[0] = new G1ANTMethodInfo(command, command.Arguments, globalArguments, family);

			for (var i = 1; i < overloads.Length; i++)
			{
				info[i] = new G1ANTMethodInfo(command, overloads[i], globalArguments, family);
			}

			return info;
		}

		public static G1ANTMethodInfo GetMethod(
			[NotNull] G1ANTRepository.CommandElement command,
			[NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
			[CanBeNull] G1ANTRepository.CommandFamilyElement family,
			[NotNull, ItemNotNull] IReadOnlyList<Type> suggested)
		{
			foreach (G1ANTMethodInfo method in ListMethods(command, globalArguments, family))
			{
				var valid = true;
				ParameterInfo[] actuals = method.GetParameters();

				// Too many parameters
				if (suggested.Count > actuals.Length)
				{
					//valid = false;
					continue;
				}

				foreach (ParameterInfo actual in actuals)
				{
					// Too few parameters
					if (actual.Position >= suggested.Count)
					{
						if (!actual.HasDefaultValue)
						{
							valid = false;
						}
					}

					// Wrong type
					else if (actual.ParameterType != suggested[actual.Position])
					{
						valid = false;
					}
				}

				if (valid)
				{
					return method;
				}
			}

			return null;
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