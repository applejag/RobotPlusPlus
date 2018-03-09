using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RobotPlusPlus.Tokenizing
{
	public class LiteralToken : Token
	{
		public object Value { get; }
		public LiteralType ValueType { get; }

		public LiteralToken(string source) : base(TokenType.Literal, source)
		{
			(ValueType, Value) = Evaluate(source);
		}

		public static (LiteralType, object value) Evaluate(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				throw new ArgumentNullException(nameof(input));

			if (input == "true") return (LiteralType.Boolean, true);
			if (input == "false") return (LiteralType.Boolean, false);
			if (input == "null") return (LiteralType.Null, null);

			if (input[0] == '"' || input[0] == '\'')
				return (LiteralType.String, Regex.Unescape(input.Substring(1, input.Length - 2)));

			if (input.IndexOf('.') == -1)
				return (LiteralType.Integer, int.Parse(input, NumberStyles.None, CultureInfo.InvariantCulture));

			return (LiteralType.Real, double.Parse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
		}

		public enum LiteralType
		{
			String,
			Integer,
			Real,
			Boolean,
			Null
		}
	}
}