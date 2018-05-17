using RobotPlusPlus.Core.Structures;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <inheritdoc />
	/// <summary>
	/// Converted from <see cref="T:RobotPlusPlus.Core.Tokenizing.Tokens.PunctuatorToken" />. Examples: x(), dialog(message: "hello")
	/// </summary>
	public class FunctionCallToken : Token
	{
		public Token LHS
		{
			get => this[0];
			set => this[0] = value;
		}

		public PunctuatorToken ParentasesGroup
		{
			get => this[1] as PunctuatorToken;
			set => this[1] = value;
		}

		public FunctionCallToken(TokenSource source, Token LHS, PunctuatorToken parentasesGroup) : base(source)
		{
			this.LHS = LHS;
			ParentasesGroup = parentasesGroup;
		}

		public override void ParseToken(IteratedList<Token> parent)
		{ }

		public override string ToString()
		{
			return LHS + base.ToString() + ParentasesGroup;
		}
	}
}