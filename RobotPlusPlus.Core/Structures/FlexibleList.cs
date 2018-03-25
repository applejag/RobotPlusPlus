using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures
{
	public class FlexibleList<T> : IList<T>, IReadOnlyList<T>
	{
		private readonly List<T> items = new List<T>();

		public int Count => items.Count;

		public bool IsReadOnly { get; } = false;

		public T this[int index]
		{
			get => index >= 0 && index < items.Count ? items[index] : default;
			set => FlexibleSetTokenAt(index, value);
		}

		private void FlexibleSetTokenAt(int index, T item)
		{
			if (index >= items.Count)
				items.AddRange(new T[index - items.Count + 1]);

			items[index] = item;
			TrimNullsAtEnd();
		}

		private void TrimNullsAtEnd()
		{
			while (items.Count > 0 && items[items.Count - 1] == null)
				items.RemoveAt(items.Count - 1);
		}

		public int IndexOf(T item)
		{
			return items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (index > items.Count)
				FlexibleSetTokenAt(index, item);
			else
				items.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			items.RemoveAt(index);
			TrimNullsAtEnd();
		}

		public void Add(T item)
		{
			TrimNullsAtEnd();
			items.Add(item);
		}

		public void Clear()
		{
			items.Clear();
		}

		public bool Contains(T item)
		{
			return items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			items.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return items.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return items.GetEnumerator();
		}

	}
}