using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileFunctionNoMatchingOverloadException : CompileFunctionException
	{
		public Type MethodDeclaringType { get; }
		public string MethodName { get; }

		[NotNull, ItemNotNull]
		public Exception[] InnerExceptions = new Exception[0];

		public CompileFunctionNoMatchingOverloadException(
			[NotNull] Type methodDeclaringType,
			[NotNull] string methodName,
			[NotNull] Token source)
			: this(methodDeclaringType, methodName, source, innerException: null)
		{
		}

		public CompileFunctionNoMatchingOverloadException(
			[NotNull] Type methodDeclaringType,
			[NotNull] string methodName,
			[NotNull] Token source,
			[CanBeNull] Exception innerException)
			: base($"Method <{methodName}> for type <{methodDeclaringType}> has no overload matching the parameters!", source, innerException)
		{
			MethodDeclaringType = methodDeclaringType;
			MethodName = methodName;
			InnerExceptions = new[] {innerException};
		}

		public CompileFunctionNoMatchingOverloadException(
			[NotNull] Type methodDeclaringType,
			[NotNull] string methodName,
			[NotNull] Token source,
			[NotNull, ItemNotNull] List<Exception> innerExceptions)
			: base($"Method <{methodName}> for type <{methodDeclaringType}> has no overload matching the parameters!\n\nOverload errors:\n{string.Join('\n', innerExceptions.Select(e => e.Message))}", source, innerExceptions.FirstOrDefault())
		{
			MethodDeclaringType = methodDeclaringType;
			MethodName = methodName;
			InnerExceptions = innerExceptions.ToArray();
		}
	}
}