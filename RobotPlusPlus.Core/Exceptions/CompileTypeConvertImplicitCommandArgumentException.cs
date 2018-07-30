using System;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Compiling.Context.Types;
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
				$"Cannot implicitly convert <{DoTheTypeThing(argument.expression.OutputType).Name ?? "null"}> to <{argumentType?.Name ?? "null"}> for argument named <{argument.name}>.",
				argument.expression.Token, DoTheTypeThing(argument.expression.OutputType), argumentType, innerException)
		{
			ArgumentName = argument.name;
		}

		private static Type DoTheTypeThing(AbstractValue val)
		{
			if (val is CSharpType cs) return cs.Type;
			return val?.GetType();
		}
	}
}