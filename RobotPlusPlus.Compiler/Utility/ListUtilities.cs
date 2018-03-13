using System.Collections;
using System.Collections.Generic;

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
	}
}