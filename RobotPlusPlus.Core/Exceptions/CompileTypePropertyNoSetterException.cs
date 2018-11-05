using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypePropertyNoSetterException : CompileTypePropertyException
	{
		public CompileTypePropertyNoSetterException(PunctuatorToken dot, Type type, string property)
			: this(dot, type, property, null)
		{ }

		public CompileTypePropertyNoSetterException(PunctuatorToken dot, Type type, string property,
			Exception innerException)
			: base(
				$"Type <{type?.Name ?? "null"}> does not allow setting the value for the property <{property ?? "null"}>.",
				dot, type, property, innerException)
		{ }
	}
}