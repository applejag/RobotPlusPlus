using System;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Reserved words. Ex: if, while, try</summary>
	public class StatementToken : Token
	{
		public Token Condition
		{
			get => this[0];
			set => this[0] = value;
		}

		public Token CodeBlock
		{
			get => this[1];
			set => this[1] = value;
		}

		public Type StatementType { get; }

		public StatementToken(TokenSource source) : base(source)
		{
			if (Enum.TryParse(SourceCode, true, out Type statementType))
				StatementType = statementType;
			else
				throw new ParseUnexpectedTokenException(this);
		}

		public override void ParseToken(IteratedList<Token> parent)
		{
			ParseTokenCondition(parent);
			ParseTokenCodeBlock(parent);
		}

		private void ParseTokenCondition(IteratedList<Token> parent)
		{
			Token next = parent.Next;

			switch (StatementType)
			{
				case Type.If:
					if (OperatorToken.ExpressionHasValue(next))
					{
						if (next.AnyRecursive(t => t is OperatorToken op && op.OperatorType == OperatorToken.Type.Assignment))
							throw new ParseTokenException($"Unexpected assignment in statement condition <{SourceCode}>.", this);

						Condition = parent.PopNext();
					}
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;
			}
		}

		private void ParseTokenCodeBlock(IteratedList<Token> parent)
		{
			Token next = parent.Next;

			switch (StatementType)
			{
				case Type.If:
					if (next is StatementToken)
						parent.ParseNextToken();

					if ((next is PunctuatorToken pun && pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases && pun.Character == '{')
						|| (next is OperatorToken op && op.OperatorType == OperatorToken.Type.Assignment)
					    || (next is StatementToken))
						CodeBlock = parent.PopNext();
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;
			}
		}

		//public override string CompileToken(Compiler compiler)
		//{
		//	var rows = new List<string>();

		//	string label = compiler.RegisterLabel("noif");

		//	compiler.assignmentNeedsCSSnipper = true;
		//	rows.Add($"jump ➜{label} if ⊂!({Condition.CompileToken(compiler)})⊃");
		//	compiler.assignmentNeedsCSSnipper = false;
		//	rows.Add(CodeBlock.CompileToken(compiler));

		//	rows.Add($"➜{label}");

		//	return string.Join('\n', rows.Where(r => !string.IsNullOrEmpty(r)));
		//}

		public enum Type
		{
			If,
			Unknown,
		}
	}
}