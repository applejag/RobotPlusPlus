using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;
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
		public readonly Converter<string, string> prefferedTransformer;

		public NameContext(IEqualityComparer<string> prefferedComparer,
			IEqualityComparer<string> generatedComparer,
			Converter<string, string> prefferedTransformer)
		{
			this.prefferedComparer = prefferedComparer;
			this.generatedComparer = generatedComparer;
			this.prefferedTransformer = prefferedTransformer;
			occupied.Push(new Dictionary<string, string>());
		}

		public NameContext()
			: this (StringComparer.InvariantCulture,
				StringComparer.CurrentCultureIgnoreCase,
				StringUtilities.EscapeIdentifier)
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

		public bool PrefferedExists([NotNull] IdentifierToken identifier)
		{
			return identifier is IdentifierTempToken || PrefferedExists(identifier.Identifier);
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

		public string GetGenerated([NotNull] IdentifierToken identifier)
		{
			if (identifier is IdentifierTempToken temp)
				return temp.GeneratedName;
			return GetGenerated(identifier.Identifier);
		}

		public string GetOrRegisterName([NotNull] string preffered)
		{	
			return GetGenerated(preffered)
			       ?? RegisterName(preffered);
		}
		
		private string GenerateName(string preffered)
		{
			preffered = prefferedTransformer(preffered);
			string generated = preffered;
			var iter = 1;

			while (GeneratedExists(generated))
			{
				generated = preffered + ++iter;
			}
			
			return generated;
		}

		public string RegisterName([NotNull] string preffered)
		{
			string generated = GenerateName(preffered);
			occupied.Peek()[preffered] = generated;
			return generated;
		}

		public string RegisterGlobalName([NotNull] string preffered)
		{
			string generated = GenerateName(preffered);
			occupied.Last()[preffered] = generated;
			return generated;
		}

		public string RegisterTempName([NotNull] string preffered)
		{
			string generated = GenerateName(preffered);
			oldGenerated.Add(generated);
			return generated;
		}

		public string GetOrRegisterName([NotNull] IdentifierToken identifier)
		{
			if (identifier is IdentifierTempToken temp)
			{
				return temp.GeneratedName = RegisterTempName("tmp");
			}

			return GetOrRegisterName(identifier.SourceCode);
		}
	}
}