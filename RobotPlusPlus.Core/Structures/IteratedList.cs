using System;
using System.Collections;
using System.Collections.Generic;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures
{
	public class IteratedList<T> : IReadOnlyList<T>
	{
		/// <summary>
		/// Tries to return previous item. If doesn't exist, will return default(<typeparamref name="T"/>)
		/// </summary>
		public T Previous => List.TryGet(Index - 1);

		/// <summary>
		/// Tries to return current item. If doesn't exist, will return default(<typeparamref name="T"/>)
		/// </summary>
		public T Current => List.TryGet(Index);

		/// <summary>
		/// Tries to return next item. If doesn't exist, will return default(<typeparamref name="T"/>)
		/// </summary>
		public T Next => List.TryGet(Index + 1);

		public int Index { get; private set; }
		public bool Reversed { get; }

		public IList<T> List { get; }

		private readonly IteratedList<T> parent = null;

		public IteratedList(IList<T> list, bool reversed = false)
			: this(list, reversed ? list.Count - 1 : 0, reversed)
		{ }

		public IteratedList(IList<T> list, int index, bool reversed = false)
		{
			List = list;
			Reversed = reversed;
			Index = index;
		}

		public IteratedList(IteratedList<T> parent, int index, bool? reversed = null)
		{
			List = parent.List;
			Reversed = reversed ?? parent.Reversed;
			Index = index;
			this.parent = parent;
		}

		/// <exception cref="IndexOutOfRangeException"></exception>
		public T PopNext()
		{
			return List.Pop(Index + 1);
		}

		/// <exception cref="IndexOutOfRangeException"></exception>
		public T PopPrevious()
		{
			return List.Pop(--Index);
		}

		/// <exception cref="IndexOutOfRangeException"></exception>
		public T PopCurrent()
		{
			return List.Pop(Index--);
		}

		public void PushNext(T value)
		{
			List.Insert(Index + 1, value);
		}

		public void PushRangeNext(params T[] values)
		{
			for (var i = 0; i < values.Length; i++)
			{
				List.Insert(Index + i + 1, values[i]);
			}
		}

		public void PushPrevious(T value)
		{
			List.Insert(Index++, value);
		}

		public void PushRangePrevious(params T[] values)
		{
			foreach (T item in values)
			{
				PushPrevious(item);
			}
		}
		public T SwapCurrent(T value)
		{
			T old = List[Index];
			List[Index] = value;
			return old;
		}

		public T SwapPrevious(T value)
		{
			T old = List[Index - 1];
			List[Index - 1] = value;
			return old;
		}

		public T SwapNext(T value)
		{
			T old = List[Index + 1];
			List[Index + 1] = value;
			return old;
		}

		public IteratedList<T> Copy()
		{
			return new IteratedList<T>(List, Reversed);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new IteratedListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count => List.Count;

		public T this[int index] => List[index];

		private sealed class IteratedListEnumerator : IEnumerator<T>
		{
			private readonly IteratedList<T> parent;

			public IteratedListEnumerator(IteratedList<T> parent)
			{
				this.parent = parent;
				Reset();
			}

			public bool MoveNext()
			{
				if (parent.Reversed)
					return (--parent.Index) >= 0;
				return (++parent.Index) < parent.Count;
			}

			public void Reset()
			{
				parent.Index = parent.Reversed ? parent.Count : -1;
			}

			public T Current => parent.Current;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}
		}
	}
}