using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	public class IdentifierTempToken : IdentifierToken
	{
		public override string Identifier => GeneratedName;

		public string GeneratedName { get; internal set; }

		public IdentifierTempToken(TokenSource source) : base(source)
		{ }
	}
}