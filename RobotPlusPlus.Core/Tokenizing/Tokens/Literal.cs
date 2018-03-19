﻿namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Numbers, strings, etc. Ex: 10.25, true, "foo"</summary>
	public abstract class Literal : Token
	{
		protected Literal(TokenSource source) : base(source)
		{ }
	}
}