using System.Text.RegularExpressions;

namespace RobotPlusPlus.Tokenizing
{
	public enum TokenType
	{
		/// <summary>Reserved words. Ex: if, while, try</summary>
		Keyword,

		/// <summary>Variables. Ex: x, myValue, go_johnny_go</summary>
		Identifier,

		/// <summary>Separators and pairing characters. Ex: }, (, ;</summary>
		Punctuators,

		/// <summary>Assignment and comparisson. Ex: =, >, +</summary>
		Operator,

		/// <summary>Numbers, strings, etc. Ex: 10.25, true, "foo"</summary>
		Literal,

		/// <summary>Ignored code. Ex: //line, /*block*/</summary>
		Comment,

		/// <summary>Spaces and newlines. Ex: \n, \t, \r</summary>
		Whitespace,
	}
}