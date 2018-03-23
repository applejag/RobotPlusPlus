using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Numbers, strings, etc. Ex: 10.25, true, "foo"</summary>
	public abstract class LiteralToken : Token
	{
		protected LiteralToken(TokenSource source) : base(source)
		{ }
	}
}