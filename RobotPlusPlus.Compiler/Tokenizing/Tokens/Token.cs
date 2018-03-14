using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Parsing;

namespace RobotPlusPlus.Tokenizing.Tokens
{
	public abstract class Token : IList<Token>, IReadOnlyList<Token>
	{
		public string SourceCode { get; }
		public int SourceLine { get; }
		public int NewLines { get; }
		public Whitespace LeadingWhitespace { get; set; }
		public Whitespace TrailingWhitespace { get; set; }

		public List<Token> Tokens { get; } = new List<Token>();
		public bool IsParsed { get; internal set; }

		protected Token(string sourceCode, int sourceLine)
		{
			SourceCode = sourceCode;
			SourceLine = sourceLine;

			var newLines = 0;
			foreach (char c in sourceCode)
			{
				if (c == '\n')
					newLines++;
			}

			NewLines = newLines;
		}

		public abstract void ParseToken(Parser parser);
		public abstract string CompileToken();

		public override string ToString()
		{
			return Tokens.Count == 0 
				? SourceCode
				: $"{SourceCode}[{string.Join(", ", Tokens.Select(t => t?.ToString() ?? "null"))}]";
		}

		#region IList implementation

		public int Count => Tokens.Count;
		public int RecursiveTokenCount => Count + Tokens.Sum(t => t?.RecursiveTokenCount ?? 0);

		public bool IsReadOnly { get; } = false;

		public Token this[int index]
		{
			get => index >= 0 && index < Tokens.Count ? Tokens[index] : null;
			set => FlexibleSetTokenAt(index, value);
		}

		private void FlexibleSetTokenAt(int index, Token item)
		{
			if (index >= Tokens.Count)
				Tokens.AddRange(new Token[index - Tokens.Count + 1]);

			Tokens[index] = item;
			TrimTokens();
		}

		public void TrimTokens()
		{
			while (Tokens.Count > 0 && Tokens[Tokens.Count-1] == null)
				Tokens.RemoveAt(Tokens.Count - 1);
		}

		public int IndexOf(Token item)
		{
			return Tokens.IndexOf(item);
		}

		public void Insert(int index, Token item)
		{
			if (index > Tokens.Count)
				FlexibleSetTokenAt(index, item);
			else
				Tokens.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Tokens.RemoveAt(index);
			TrimTokens();
		}

		public void Add(Token item)
		{
			Tokens.Add(item);
		}

		public void Clear()
		{
			Tokens.Clear();
		}

		public bool Contains(Token item)
		{
			return Tokens.Contains(item);
		}

		public bool ContainsRecursive(Token item)
		{
			return Contains(item) || Tokens.Any(t => t.ContainsRecursive(item));
		}

		public void CopyTo(Token[] array, int arrayIndex)
		{
			Tokens.CopyTo(array, arrayIndex);
		}

		public bool Remove(Token item)
		{
			return Tokens.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Token> GetEnumerator()
		{
			return Tokens.GetEnumerator();
		}

		#endregion
	}
}