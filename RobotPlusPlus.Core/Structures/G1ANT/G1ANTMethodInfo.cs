using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Utility;

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

		internal static G1ANTMethodInfo[] ListMethods(
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

		public static bool MethodMatches(MethodInfo method, CommandUnit.Argument[] args)
		{
			// TODO: Check named arguments, not only indexed
			ParameterInfo[] actuals = method.GetParameters();

			// Too many parameters
			if (args.Length > actuals.Length)
			{
				return false;
			}

			foreach (ParameterInfo actual in actuals)
			{
				// Too few parameters
				if (actual.Position >= args.Length)
				{
					if (!actual.HasDefaultValue)
					{
						return false;
					}
				}

				// Wrong type
				else
				{
					CSharpType abstractValue = (CSharpType) args[actual.Position].expression.OutputType;
					if (!TypeChecking.CanImplicitlyConvert(abstractValue.Type, actual.ParameterType))
					{
						return false;
					}
				}
			}

			return true;
		}

		[CanBeNull]
		public static MethodInfo GetMethod(MethodInfo[] methodInfos, params CommandUnit.Argument[] args)
		{
			return methodInfos
				.TryFirst(m => MethodMatches(m, args), out MethodInfo met)
				? met : null;
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