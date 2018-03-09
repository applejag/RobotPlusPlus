using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RobotPlusPlus.Tokenizing
{
	public class LiteralToken : Token
	{
		public object Value { get; }
		public ValueType LiteralType { get; }

		public LiteralToken(string sourceCode, int sourceLine) : base(TokenType.Literal, sourceCode, sourceLine)
		{
			(LiteralType, Value) = Evaluate(sourceCode);
		}

		public static (ValueType, object value) Evaluate(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				throw new ArgumentNullException(nameof(input));

			if (input == "true") return (ValueType.Boolean, true);
			if (input == "false") return (ValueType.Boolean, false);
			if (input == "null") return (ValueType.Null, null);

			if (input[0] == '"' || input[0] == '\'')
				return (ValueType.String, Regex.Unescape(input.Substring(1, input.Length - 2)));

			if (input.IndexOf('.') == -1)
				return (ValueType.Integer, int.Parse(input, NumberStyles.None, CultureInfo.InvariantCulture));

			return (ValueType.Real, double.Parse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
		}
	}
}