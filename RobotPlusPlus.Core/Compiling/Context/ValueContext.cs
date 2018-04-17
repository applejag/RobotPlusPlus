using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.Context
{
	public class ValueContext
	{
		private readonly Stack<HashSet<AbstractValue>> layers = new Stack<HashSet<AbstractValue>>();
		private readonly HashSet<AbstractValue> decayed = new HashSet<AbstractValue>();

		public int Layers => layers.Count;
		public IReadOnlyCollection<AbstractValue> DecayedValues => decayed;

		public IEqualityComparer<string> generatedComparer = StringComparer.CurrentCultureIgnoreCase;
		public IEqualityComparer<string> prefferedComparer = StringComparer.InvariantCulture;
		public Converter<string, string> identifierTransformer = StringUtilities.EscapeIdentifier;

		public ValueContext()
		{
			layers.Push(new HashSet<AbstractValue>());
		}

		public void PushLayer()
		{
			layers.Push(new HashSet<AbstractValue>());
		}

		public void PopLayer()
		{
			if (layers.Count <= 1)
				throw new InvalidOperationException("You can't pop the top layer.");

			foreach (AbstractValue poppedValue in layers.Pop())
			{
				decayed.Add(poppedValue);
			}
		}

		[Pure]
		public bool IdentifierExists([NotNull] string identifier)
		{
			return layers.Any(layer => layer.ContainsIdentifier(identifier, prefferedComparer));
		}

		[Pure]
		public bool GeneratedExists([NotNull] string generated)
		{
			return decayed.ContainsGenerated(generated, generatedComparer)
				   || layers.Any(layer => layer.ContainsGenerated(generated, generatedComparer));
		}

		[CanBeNull, Pure]
		public AbstractValue FindValue([NotNull] string identifier)
		{
			foreach (HashSet<AbstractValue> layer in layers)
			{
				if (layer.TryFirst(k => prefferedComparer.Equals(k.Identifier, identifier), out AbstractValue key))
					return key;
			}

			return null;
		}

		[NotNull, Pure]
		public string GenerateName([NotNull] string identifier)
		{
			identifier = identifierTransformer(identifier);
			string generated = identifier;
			var iter = 1;

			while (GeneratedExists(generated))
			{
				generated = identifier + ++iter;
			}

			return generated;
		}

		[NotNull]
		public TValue RegisterValue<TValue>([NotNull] TValue value)
			where TValue : AbstractValue
		{
			if (value is null) throw new ArgumentNullException(nameof(value));
			layers.Peek().Add(value);
			return value;
		}

		[NotNull]
		public TValue RegisterValueGlobally<TValue>([NotNull] TValue value)
			where TValue : AbstractValue
		{
			if (value is null) throw new ArgumentNullException(nameof(value));
			layers.Last().Add(value);
			return value;
		}

		[NotNull]
		public TValue RegisterValueDecayed<TValue>([NotNull] TValue value)
			where TValue : AbstractValue
		{
			if (value is null) throw new ArgumentNullException(nameof(value));
			decayed.Add(value);
			return value;
		}

		//public string RegisterName([NotNull] string identifier)
		//{
		//	string generated = GenerateName(identifier);
		//	layers.Peek()[identifier] = generated;
		//	return generated;
		//}

		//public string RegisterGlobalName([NotNull] string identifier)
		//{
		//	string generated = GenerateName(identifier);
		//	layers.Last()[identifier] = generated;
		//	return generated;
		//}

		//public string RegisterTempName([NotNull] string identifier)
		//{
		//	string generated = GenerateName(identifier);
		//	oldGenerated.Add(generated);
		//	return generated;
		//}

		//public string GetOrRegisterName([NotNull] IdentifierToken identifier)
		//{
		//	if (identifier is IdentifierTempToken temp)
		//	{
		//		return temp.GeneratedName = RegisterTempName("tmp");
		//	}

		//	return GetOrRegisterName(identifier.SourceCode);
		//}
	}
}