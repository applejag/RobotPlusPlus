using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileParameterNamedDoesntExistException : CompileParameterException
	{
		[NotNull]
		public string ParameterName { get; }

		public CompileParameterNamedDoesntExistException(
			[NotNull] MethodInfo method,
			[NotNull] string name,
			[NotNull] Token source)
			: this(method, name, source, null)
		{
		}

		public CompileParameterNamedDoesntExistException(
			[NotNull] MethodInfo method,
			[NotNull] string name,
			[NotNull] Token source,
			[CanBeNull] Exception innerException)
			: base($"Method <{method.Name}> does not contain a parameter named by <{name}>.",
				method, source, innerException)
		{
			ParameterName = name;
		}
	}
}