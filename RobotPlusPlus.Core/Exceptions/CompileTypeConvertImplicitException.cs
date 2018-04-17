using System;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
	public class CompileTypeConvertImplicitException : CompileException
	{
		public Type ToType { get; }
		public Type FromType { get; }

		public CompileTypeConvertImplicitException(Token expression, Type from, Type to)
			: this(expression, @from, to, null)
		{ }

		public CompileTypeConvertImplicitException(Token expression, Type from, Type to,
			Exception innerException)
			: this(
				$"Cannot implicitly convert <{from.Name}> to <{to.Name}> in expression.",
				expression, from, to, innerException)
		{ }

		protected CompileTypeConvertImplicitException(string message, Token expression, Type from, Type to,
			Exception innerException)
			: base(message, expression, innerException)
		{
			ToType = to;
			FromType = from;
		}
	}
}