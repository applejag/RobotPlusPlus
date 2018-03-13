using System.Collections.Generic;
using System.Linq;

namespace RobotPlusPlus
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

				if (!this.Any(x => x.Value.Equals(index)))
					throw new KeyNotFoundException();
				return this.First(x => x.Value.Equals(index)).Key;
			}
		}
	}
}