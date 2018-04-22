using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class AbstractValue
	{
		[NotNull]
		public string Identifier { get; }
		[NotNull]
		public string Generated { get; }
		
		protected AbstractValue(
			[NotNull] string generated,
			[NotNull] string identifier)
		{
			Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
			Generated = generated ?? throw new ArgumentNullException(nameof(generated));
		}

		[Pure]
		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case string str when str == Generated:
				case AbstractValue value when Generated == value.Generated:
					return true;

				default:
					return false;
			}
		}

		[NotNull, Pure]
		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(Identifier))
				return string.IsNullOrWhiteSpace(Generated) ? "<null>" : Generated;
			return string.IsNullOrWhiteSpace(Generated) ? Identifier : $"{Identifier}<{Generated}>";
		}

		[Pure]
		public override int GetHashCode()
		{
			return unchecked(1890376855 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Generated));
		}

		[Pure]
		public static bool operator ==(AbstractValue value1, AbstractValue value2)
		{
			return EqualityComparer<AbstractValue>.Default.Equals(value1, value2);
		}

		[Pure]
		public static bool operator !=(AbstractValue value1, AbstractValue value2)
		{
			return !(value1 == value2);
		}
	}
}