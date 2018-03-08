using System.Text.RegularExpressions;
using RobotPlusPlus.Attributes;

namespace RobotPlusPlus.Tokenizing
{
	public enum TokenType
	{
		/// <summary>Reserved words. Ex: if, while, try</summary>
		[TokenWordInList("if","while","try")]
		Keyword,

		/// <summary>Variables. Ex: x, myValue, go_johnny_go</summary>
		[TokenRegex(@"[\p{L}_][\p{L}_\p{N}]*")]
		Identifier,

		/// <summary>Separators and pairing characters. Ex: }, (, ;</summary>
		[TokenRegex(@"")]
		Punctuators,

		/// <summary>Assignment and comparisson. Ex: =, >, +</summary>
		[TokenRegex(@"(=|\+=|-=)")]
		Operator,

		/// <summary>Numbers, strings, etc. Ex: 10.25, true, "foo"</summary>
		[TokenRegex(@"")]
		Literal,

		/// <summary>Ignored code. Ex: //line, /*block*/</summary>
		[TokenRegex(@"(\/\/.*|/*)")]
		Comment,

		/// <summary>Spaces and newlines. Ex: \n, \t, \r</summary>
		[TokenRegex(@"\s+")]
		Whitespace,
	}
}