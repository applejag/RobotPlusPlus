using System.Collections.Generic;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.Asserting.Definitions
{
	public class Variable
	{
		public string Name { get; }
		public Token InitialToken { get; }
		public ValueType HeldValue { get; private set; } = ValueType.Null;

		public Variable(Token source)
		{
			InitialToken = source;
			Name = source.SourceCode;
		}

		public void SetValueType(ValueType valueType, int line)
		{
			if (HeldValue != valueType && HeldValue != ValueType.Null)
				throw new ParseException($"Variable <{Name}> is already of type <{HeldValue}>; it cannot be changed to <{valueType}>!", line);

			HeldValue = valueType;
		}

		public override bool Equals(object obj)
		{
			return obj is Variable variable &&
				   Name == variable.Name;
		}

		public override int GetHashCode()
		{
			return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
		}
	}
}