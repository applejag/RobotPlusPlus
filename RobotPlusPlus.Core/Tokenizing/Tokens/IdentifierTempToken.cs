using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	public class IdentifierTempToken : IdentifierToken
	{
		public override string Identifier => GeneratedName;

		public IdentifierTempToken(TokenSource source) : base(source)
		{ }

		public override string ToString()
		{
			return "$tmp";
		}
	}
}