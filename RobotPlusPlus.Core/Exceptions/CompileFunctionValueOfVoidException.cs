using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileFunctionValueOfVoidException : CompileFunctionException
	{
		public MethodInfo MethodInfo { get; }

		public CompileFunctionValueOfVoidException(
			[NotNull] MethodInfo methodInfo,
			[NotNull] Token source)
			: this(methodInfo, source, innerException: null)
		{
		}

		public CompileFunctionValueOfVoidException(
		    [NotNull] MethodInfo methodInfo,
            [NotNull] Token source,
			[CanBeNull] Exception innerException)
			: base($"Method use value from <{methodInfo.Name}> as it returns void!", source, innerException)
		{
			MethodInfo = methodInfo;
		}
	}
}