using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Structures
{
	public class NameContext
	{
		private readonly Stack<Dictionary<string,string>> occupied = new Stack<Dictionary<string,string>>();
		private readonly List<string> oldGenerated = new List<string>();

		public int Layers => occupied.Count;

		public readonly IEqualityComparer<string> comparer;

		public NameContext(IEqualityComparer<string> comparer)
		{
			this.comparer = comparer;
			occupied.Push(new Dictionary<string, string>(comparer));
		}

		public NameContext()
			: this (StringComparer.OrdinalIgnoreCase)
		{}

		public void PushLayer()
		{
			occupied.Push(new Dictionary<string, string>(comparer));
		}

		public void PopLayer()
		{
			if (occupied.Count <= 1)
				throw new InvalidOperationException("You can't pop the top layer.");
			
			oldGenerated.AddRange(occupied.Pop().Values);
		}

		public bool PrefferedExists([NotNull] string preffered)
		{
			return occupied.Any(layer => layer.ContainsKey(preffered));
		}

		public bool GeneratedExists([NotNull] string generated)
		{
			return oldGenerated.Contains(generated) || occupied.Any(layer => layer.ContainsValue(generated));
		}

		public string GetGenerated([NotNull] string preffered)
		{
			return occupied.FirstOrDefault(d => d.ContainsKey(preffered))?[preffered];
		}

		public string GetOrGenerateName([NotNull] string preffered)
		{
			return GetGenerated(preffered)
			       ?? GenerateName(preffered);
		}

		public string GenerateName([NotNull] string preffered)
		{
			string generated = preffered;
			var iter = 1;

			while (GeneratedExists(generated))
			{
				generated = preffered + ++iter;
			}

			occupied.Peek()[preffered] = generated;

			return generated;
		}
	}
}