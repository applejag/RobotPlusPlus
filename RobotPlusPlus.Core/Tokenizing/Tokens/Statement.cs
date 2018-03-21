﻿using System;
using System.Collections.Generic;
using System.Linq;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Tokenizing.Tokens
{
	/// <summary>Reserved words. Ex: if, while, try</summary>
	public class Statement : Token
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

		public Statement(TokenSource source) : base(source)
		{
			if (Enum.TryParse(SourceCode, true, out Type statementType))
				StatementType = statementType;
			else
				throw new ParseUnexpectedTokenException(this);
		}

		public override void ParseToken(IList<Token> parent, int myIndex)
		{
			ParseTokenCondition(parent, myIndex);
			ParseTokenCodeBlock(parent, myIndex);
		}

		private void ParseTokenCondition(IList<Token> parent, int myIndex)
		{
			int nextIndex = myIndex + 1;
			Token next = parent.TryGet(nextIndex);

			switch (StatementType)
			{
				case Type.If:
					if (Operator.ExpressionHasValue(next))
					{
						if (next.AnyRecursive(t => t is Operator op && op.OperatorType == Operator.Type.Assignment))
							throw new ParseTokenException($"Unexpected assignment in statement condition <{SourceCode}>.", this);

						Condition = parent.Pop(nextIndex);
					}
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;
			}
		}

		private void ParseTokenCodeBlock(IList<Token> parent, int myIndex)
		{
			int nextIndex = myIndex + 1;
			Token next = parent.TryGet(nextIndex);

			switch (StatementType)
			{
				case Type.If:
					if (next is Statement)
						parent.ParseTokenAt(nextIndex);

					if ((next is Punctuator pun && pun.PunctuatorType == Punctuator.Type.OpeningParentases && pun.Character == '{')
						|| (next is Operator op && op.OperatorType == Operator.Type.Assignment)
					    || (next is Statement))
						CodeBlock = parent.Pop(nextIndex);
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;
			}
		}

		public override string CompileToken(Compiler compiler)
		{
			var rows = new List<string>();

			string label = compiler.RegisterLabel("noif");

			compiler.assignmentNeedsCSSnipper = true;
			rows.Add($"jump ➜{label} if ⊂!({Condition.CompileToken(compiler)})⊃");
			compiler.assignmentNeedsCSSnipper = false;
			rows.Add(CodeBlock.CompileToken(compiler));

			rows.Add($"➜{label}");

			return string.Join('\n', rows.Where(r => !string.IsNullOrEmpty(r)));
		}

		public enum Type
		{
			If,
			Unknown,
		}
	}
}