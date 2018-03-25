using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures
{
	public class NameContext
	{
		private readonly Stack<Dictionary<string,string>> occupied = new Stack<Dictionary<string,string>>();
		private readonly List<string> oldGenerated = new List<string>();

		public int Layers => occupied.Count;

		public readonly IEqualityComparer<string> generatedComparer;
		public readonly IEqualityComparer<string> prefferedComparer;

		public NameContext(IEqualityComparer<string> prefferedComparer,
			IEqualityComparer<string> generatedComparer)
		{
			this.prefferedComparer = prefferedComparer;
			this.generatedComparer = generatedComparer;
			occupied.Push(new Dictionary<string, string>());
		}

		public NameContext()
			: this (StringComparer.InvariantCulture, StringComparer.CurrentCultureIgnoreCase)
		{}

		public void PushLayer()
		{
			occupied.Push(new Dictionary<string, string>());
		}

		public void PopLayer()
		{
			if (occupied.Count <= 1)
				throw new InvalidOperationException("You can't pop the top layer.");
			
			oldGenerated.AddRange(occupied.Pop().Values);
		}

		public bool PrefferedExists([NotNull] string preffered)
		{
			return occupied.Any(layer => layer.Keys.Contains(preffered, prefferedComparer));
		}

		public bool GeneratedExists([NotNull] string generated)
		{
			return oldGenerated.Contains(generated, generatedComparer)
			       || occupied.Any(layer => layer.Values.Contains(generated, generatedComparer));
		}

		public string GetGenerated([NotNull] string preffered)
		{
			foreach (Dictionary<string, string> layer in occupied)
			{
				if (layer.Keys.TryFirst(k => prefferedComparer.Equals(k, preffered), out string key))
					return layer[key];
			}

			return null;
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