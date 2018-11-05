using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileParameterIndexedDoesntExistException : CompileParameterException
	{
		public int ParameterIndex { get; }

		public CompileParameterIndexedDoesntExistException(
			[NotNull] MethodInfo method,
			int index,
			[NotNull] Token source)
			: this(method, index, source, null)
		{
		}

		public CompileParameterIndexedDoesntExistException(
			[NotNull] MethodInfo method,
			int index,
			[NotNull] Token source,
			[CanBeNull] Exception innerException)
			: base($"Method <{StringifyMethod(method)}> does not accept <{index+1}> parameters.",
				method, source, innerException)
		{
			ParameterIndex = index;
		}
	}
}