using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Structures
{
	public class RowBuilder : IReadOnlyList<string>
	{
		private readonly List<string> rows;

		public int Count => rows.Count;

		public string this[int index] => rows[index];

		public RowBuilder()
		{
			rows = new List<string>();
		}

		public RowBuilder(int capacity)
		{
			rows = new List<string>(capacity);
		}

		public RowBuilder(IEnumerable<string> collection)
		{
			rows = new List<string>(collection);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(string text)
		{
			if (ValidLine(text))
				rows.Add(text);
		}

		[StringFormatMethod("format")]
		public void AppendLine(string format, params object[] args)
		{
			AppendLine(string.Format(format, args));
		}

		[StringFormatMethod("format")]
		public void AppendLine(IFormatProvider provider,string format, params object[] args)
		{
			AppendLine(string.Format(provider, format, args));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(string text)
		{
			int lastIndex = rows.Count - 1;
			if (rows.Count == 0)
			{
				rows.Add(text);
				lastIndex++;
			}
			else
				rows[lastIndex] += text;

			if (!ValidLine(rows[lastIndex]))
				rows.RemoveAt(lastIndex);
		}

		[StringFormatMethod("format")]
		public void Append(string format, params object[] args)
		{
			Append(string.Format(format, args));
		}

		[StringFormatMethod("format")]
		public void Append(IFormatProvider provider, string format, params object[] args)
		{
			Append(string.Format(provider, format, args));
		}

		public override string ToString()
		{
			return string.Join('\n', this);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return rows.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static bool ValidLine(string line)
		{
			return !string.IsNullOrWhiteSpace(line);
		}
	}
}