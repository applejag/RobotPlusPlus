using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context.Types;

namespace RobotPlusPlus.Core.Compiling.Context
{
	public static class ValueListExtensions
	{
		[Pure]
		public static bool ContainsIdentifier([NotNull] this IEnumerable<AbstractValue> list, string identifier)
		{
			return list.Any(v => v.Identifier == identifier);
		}

		[Pure]
		public static bool ContainsIdentifier([NotNull] this IEnumerable<AbstractValue> list, string identifier, IEqualityComparer<string> comparer)
		{
			return list.Any(v => comparer.Equals(v.Identifier, identifier));
		}

		[Pure]
		public static bool ContainsGenerated([NotNull] this IEnumerable<AbstractValue> list, string generated)
		{
			return list.Any(v => v.Generated == generated);
		}

		[Pure]
		public static bool ContainsGenerated([NotNull] this IEnumerable<AbstractValue> list, string generated, IEqualityComparer<string> comparer)
		{
			return list.Any(v => comparer.Equals(v.Generated, generated));
		}
	}
}