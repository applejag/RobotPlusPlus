using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Utility
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

		public static bool TryFirstRecursive<TSource>(this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate,
			out TSource value)
			where TSource : IEnumerable<TSource>
		{
			foreach (TSource item in source)
			{
				if (TryFirstRecursive(item, predicate, out value, true))
					return true;
			}

			value = default;
			return false;
		}

		public static bool TryFirstRecursive<TSource>(this TSource source, [NotNull] Func<TSource, bool> predicate,
			out TSource value, bool includeTop)
			where TSource : IEnumerable<TSource>
		{
			if (includeTop && predicate(source))
			{
				value = source;
				return true;
			}

			return TryFirstRecursive(source, predicate, out value);
		}

		public static bool AnyRecursive<TSource>([NotNull] this IEnumerable<TSource> source,
			[NotNull] Func<TSource, bool> predicate)
			where TSource : IEnumerable<TSource>
		{
			return source.Any(item => predicate(item) || item.AnyRecursive(predicate));
		}

		public static bool AllRecursive<TSource>([NotNull] this IEnumerable<TSource> source,
			[NotNull] Func<TSource, bool> predicate)
			where TSource : IEnumerable<TSource>
		{
			return source.All(item => predicate(item) && item.AllRecursive(predicate));
		}

		public static bool AnyRecursive<TSource>([NotNull] this TSource source,
			[NotNull] Func<TSource, bool> predicate, bool includeTop)
			where TSource : IEnumerable<TSource>
		{
			if (includeTop && predicate(source)) return true;
			return source.AnyRecursive(predicate);
		}

		public static bool AllRecursive<TSource>([NotNull] this TSource source,
			[NotNull] Func<TSource, bool> predicate, bool includeTop)
			where TSource : IEnumerable<TSource>
		{
			if (includeTop) return predicate(source) && source.All(item => predicate(item) && item.AllRecursive(predicate));
			return source.AllRecursive(predicate);
		}

		public static int CountRecursive<TSource>([NotNull] this IEnumerable<TSource> source)
			where TSource : IEnumerable<TSource>
		{
			return source.Sum(item => 1 + item?.CountRecursive() ?? 0);
		}

		public static int CountRecursive<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate)
			where TSource : IEnumerable<TSource>
		{
			return source.Sum(item => predicate(item) ? 1 + item?.CountRecursive(predicate) ?? 0 : 0);
		}

		public static bool ContainsRecursive<TSource>([NotNull] this IEnumerable<TSource> source, TSource value)
			where TSource : IEnumerable<TSource>
		{
			return source.Any(t => EqualityComparer<TSource>.Default.Equals(t, value) || t.ContainsRecursive(value));
		}

		public static bool ContainsRecursive<TSource>([NotNull] this IEnumerable<TSource> source, TSource value, [NotNull] IEqualityComparer<TSource> comparer)
			where TSource : IEnumerable<TSource>
		{
			return source.Any(t => comparer.Equals(t, value) || t.ContainsRecursive(value, comparer));
		}

		public static TSource FirstOrDefaultRecursive<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate)
			where TSource : IEnumerable<TSource>
		{
			TryFirstRecursive(source, predicate, out TSource value);
			return value;
		}

		public static TSource FirstOrDefaultRecursive<TSource>([NotNull] this TSource source, [NotNull] Func<TSource, bool> predicate,
			bool includeTop)
			where TSource : IEnumerable<TSource>
		{
			TryFirstRecursive(source, predicate, out source, includeTop);
			return source;
		}

		public static TSource FirstRecursive<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate)
			where TSource : IEnumerable<TSource>
		{
			if (!TryFirstRecursive(source, predicate, out TSource value))
				throw new InvalidOperationException("First found no match.");
			return value;
		}

		public static TSource FirstRecursive<TSource>([NotNull] this TSource source, [NotNull] Func<TSource, bool> predicate,
			bool includeTop)
			where TSource : IEnumerable<TSource>
		{
			if (!TryFirstRecursive(source, predicate, out source, includeTop))
				throw new InvalidOperationException("First found no match.");
			return source;
		}

		public static void ForEachRecursive<TSource>([NotNull] this IEnumerable<TSource> source,
			[NotNull] Action<TSource> callback)
			where TSource : IEnumerable<TSource>
		{
			foreach (TSource item in source)
			{
				callback(item);
				item?.ForEachRecursive(callback);
			}
		}

		public static void ForEachRecursive<TSource>([NotNull] this TSource source,
			[NotNull] Action<TSource> callback, bool includeTop)
			where TSource : IEnumerable<TSource>
		{
			if (includeTop)
				callback(source);

			source.ForEachRecursive(callback);
		}

		/// <summary>
		/// Will return the value at <paramref name="source"/>[<paramref name="index"/>].
		/// If index is out of bounds, will return default(<typeparamref name="TSource"/>)
		/// </summary>
		public static TSource TryGet<TSource>(this IList<TSource> source, int index)
		{
			if (index >= 0 && index < source.Count)
				return source[index];
			return default;
		}

		/// <summary>
		/// Tries to set out parameter <paramref name="value"/> to the value at <paramref name="source"/>[<paramref name="index"/>].
		/// If index is out of bounds, will return false. Else will return true.
		/// </summary>
		public static bool TryGet<TSource>(this IList<TSource> source, int index, out TSource value)
		{
			if (index >= 0 && index < source.Count)
			{
				value = source[index];
				return true;
			}
			value = default;
			return false;
		}
		
		public static TSource Pop<TSource>(this IList<TSource> source, int index)
		{
			if (index < 0 || index >= source.Count)
				throw new IndexOutOfRangeException();

			TSource value = source[index];
			source.RemoveAt(index);
			return value;
		}
	}
}