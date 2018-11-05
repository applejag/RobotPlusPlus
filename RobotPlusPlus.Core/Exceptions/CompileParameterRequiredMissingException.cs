using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileParameterRequiredMissingException : CompileParameterException
	{
		public int ParameterIndex { get; }
		[NotNull]
		public string ParameterName { get; }

		public CompileParameterRequiredMissingException(
			[NotNull] MethodInfo method,
			[NotNull] ParameterInfo param,
			[NotNull] Token source)
			: this(method, param, source, null)
		{
		}

		public CompileParameterRequiredMissingException(
			[NotNull] MethodInfo method,
			[NotNull] ParameterInfo param,
			[NotNull] Token source, 
			[CanBeNull] Exception innerException) 
			: base($"Method <{method.Name}> is missing required parameter <{param.Name}>.",
				method, source, innerException)
		{
			ParameterIndex = param.Position;
			ParameterName = param.Name;
		}
	}
}