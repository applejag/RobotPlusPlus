using System.Collections.Generic;
using RobotPlusPlus.Linguist.Utility;

namespace RobotPlusPlus.Linguist
{
	public class Bictionary<T1, T2> : Dictionary<T1, T2>
	{
		public T1 this[T2 index]
		{
			get
			{
				if (index is T1 key
					&& TryGetValue(key, out T2 value)
					&& value is T1 item)
					return item;
				
				if (this.TryFirst(x => x.Value.Equals(index), out KeyValuePair<T1, T2> pair))
					return pair.Key;

				throw new KeyNotFoundException();
			}
		}
	}
}