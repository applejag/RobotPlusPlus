using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypePropertyNoGetterException : CompileTypePropertyException
	{
		public CompileTypePropertyNoGetterException(PunctuatorToken dot, Type type, string property)
			: this(dot, type, property, null)
		{ }

		public CompileTypePropertyNoGetterException(PunctuatorToken dot, Type type, string property,
			Exception innerException)
			: base(
				$"Type <{type?.Name ?? "null"}> does not allow getting the value from the property <{property ?? "null"}>.",
				dot, type, property, innerException)
		{ }
	}
}