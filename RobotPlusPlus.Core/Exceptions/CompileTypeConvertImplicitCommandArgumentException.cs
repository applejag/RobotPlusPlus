using System;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeConvertImplicitCommandArgumentException : CompileTypeConvertImplicitException
	{
		public string ArgumentName { get; }

		public CompileTypeConvertImplicitCommandArgumentException(CommandUnit.NamedArgument argument, Type argumentType)
			: this(argument, argumentType, null)
		{ }

		public CompileTypeConvertImplicitCommandArgumentException(CommandUnit.NamedArgument argument, Type argumentType,
			Exception innerException)
			: base(
				$"Cannot implicitly convert <{argument.expression.OutputType?.Name ?? "null"}> to <{argumentType?.Name ?? "null"}> for argument named <{argument.name}>.",
				argument.expression.Token, argument.expression.OutputType, argumentType, innerException)
		{
			ArgumentName = argument.name;
		}
	}
}