using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypePropertyDoesNotExistException : CompileTypePropertyException
	{
		public CompileTypePropertyDoesNotExistException(PunctuatorToken dot, Type type, string property)
			: this(dot, type, property, null)
		{ }

		public CompileTypePropertyDoesNotExistException(PunctuatorToken dot, Type type, string property,
			Exception innerException)
			: base(
				$"Type <{type?.Name ?? "null"}> does not contain the property <{property ?? "null"}>.",
				dot, type, property, innerException)
		{ }
	}
}