using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileParameterException : CompileFunctionException
	{
		[NotNull]
		public MethodInfo Method { get; }

		public CompileParameterException(
			[NotNull] string msg,
			[NotNull] MethodInfo method,
			[NotNull] Token source) 
			: this(msg, method, source, null)
		{
		}

		public CompileParameterException(
			[NotNull] string msg,
			[NotNull] MethodInfo method,
			[NotNull] Token source,
			[CanBeNull] Exception innerException) 
			: base(msg, source, innerException)
		{
			Method = method;
		}
	}
}