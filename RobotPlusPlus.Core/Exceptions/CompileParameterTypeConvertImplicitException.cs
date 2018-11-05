using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileParameterTypeConvertImplicitException : CompileParameterException
	{
		public int ParameterIndex { get; }
		[NotNull]
		public string ParameterName { get; }
		[NotNull]
		public Type ParameterType { get; }

		[CanBeNull]
		public Type ArgumentType { get; }

		public CompileParameterTypeConvertImplicitException(
			[NotNull] MethodInfo method,
			[CanBeNull] Type argumentType,
			[NotNull] ParameterInfo param,
			[NotNull] Token source)
			: this(method, argumentType, param, source, null)
		{
		}

		public CompileParameterTypeConvertImplicitException(
			[NotNull] MethodInfo method,
			[CanBeNull] Type argumentType,
			[NotNull] ParameterInfo param,
			[NotNull] Token source,
			[CanBeNull] Exception innerException)
			: base($"Cannot implicitly convert <{argumentType?.Name ?? "null"}> to <{param.ParameterType?.Name ?? "name"}> for parameter <{param.Name}> for method <{method.Name}>.",
				method, source, innerException)
		{
			ParameterIndex = param.Position;
			ParameterName = param.Name;
			ParameterType = param.ParameterType;
			ArgumentType = argumentType;
		}

	}
}