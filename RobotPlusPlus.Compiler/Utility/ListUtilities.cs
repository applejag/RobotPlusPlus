using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace RobotPlusPlus.Utility
{
	public static class ListUtilities
	{
		public static TList GetRangeAfter<TList>(this TList list, int index, bool includeIndexed = true)
			where TList : IList, new()
		{
			var range = new TList();

			if (!includeIndexed) index++;

			for (; index < list.Count; index++)
			{
				range.Add(list[index]);
			}

			return range;
		}

		public static TList GetRangeBefore<TList>(this TList list, int index, bool includeIndexed = true)
			where TList : IList, new()
		{
			var range = new TList();

			if (includeIndexed) index++;

			for (var i = 0; i < index; i++)
			{
				range.Add(list[i]);
			}
			
			return range;
		}
		
		public static TList PopRangeAfter<TList>(this TList list, int index, bool includeIndexed = true)
			where TList : IList, new()
		{
			var range = new TList();

			if (!includeIndexed) index++;

			while (index < list.Count)
			{
				range.Add(list[index]);
				list.RemoveAt(index);
			}

			return range;
		}

		public static bool TryFirst<TSource>(this IEnumerable<TSource> source, out TSource value)
		{
			foreach (TSource item in source)
			{
				value = item;
				return true;
			}

			value = default;
			return false;
		}

		public static bool TryFirst<TSource>(this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate, out TSource value)
		{
			foreach (TSource item in source)
			{
				if (!predicate(item)) continue;

				value = item;
				return true;
			}

			value = default;
			return false;
		}

		public static bool AnyRecursive<TSource>(this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate)
			where TSource : IEnumerable<TSource>
		{
			return source.Any(item => predicate(item) || item.AnyRecursive(predicate));
		}
	}
}