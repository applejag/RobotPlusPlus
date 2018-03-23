using System.Collections.Generic;
using System.Globalization;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens.Literals
{
	public class LiteralNumberToken : LiteralToken
	{
		/// <summary><see cref="Value"/> is <seealso cref="double"/></summary>
		public bool IsReal => Value is double;

		public double RealValue => (double) Value;

		/// <summary><see cref="Value"/> is <seealso cref="int"/></summary>
		public bool IsInteger => Value is int;

		public int IntegerValue => (int) Value;

		public object Value { get; }

		public LiteralNumberToken(TokenSource source) : base(source)
		{
			if (SourceCode.EndsWith('f', ignoreCase: true))
				Value = double.Parse(SourceCode.Substring(0, SourceCode.Length - 1), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
			else if (SourceCode.IndexOf('.') == -1)
				Value = int.Parse(SourceCode, NumberStyles.None, CultureInfo.InvariantCulture);
			else
				Value = double.Parse(SourceCode, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
		}

		public override void ParseToken(IList<Token> parent, int myIndex)
		{ }

		public override string CompileToken(Compiler compiler)
		{
			return IsReal
				? RealValue.ToString("0.0#############################", CultureInfo.InvariantCulture)
				: IntegerValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}