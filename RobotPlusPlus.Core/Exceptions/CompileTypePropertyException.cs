using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypePropertyException : CompileException
	{
		public Type Type { get; }
		public string Property { get; }

		public CompileTypePropertyException(string message, PunctuatorToken dot, Type type, string property)
			: this(message, dot, type, property, null)
		{ }

		public CompileTypePropertyException(string message, PunctuatorToken dot, Type type, string property,
			Exception innerException)
			: base(message, dot, innerException)
		{
			Type = type;
			Property = property;
		}
	}
}