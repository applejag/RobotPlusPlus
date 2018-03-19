using System;
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
		public const int _Condition = 0;
		public const int _CodeBlock = 1;

		public Token Condition => this[_Condition];
		public Token CodeBlock => this[_CodeBlock];
		public Type StatementType { get; }

		public Statement(string sourceCode, int sourceLine) : base(sourceCode, sourceLine)
		{
			if (Enum.TryParse(sourceCode, true, out Type statementType))
				StatementType = statementType;
			else
				throw new ParseUnexpectedTokenException(this);
		}

		public override void ParseToken(Parser parser)
		{
			ParseTokenCondition(parser);
			ParseTokenCodeBlock(parser);
		}

		private void ParseTokenCondition(Parser parser)
		{
			Token next = parser.NextToken;
			switch (StatementType)
			{
				case Type.If:
					if (Operator.ExpressionHasValue(next))
					{
						if (next.AnyRecursive(t => t is Operator op && op.OperatorType == Operator.Type.Assignment))
							throw new ParseTokenException($"Unexpected assignment in statement condition <{SourceCode}>.", this);
						parser.TakeNextToken(_Condition);
					}
					else
						throw new ParseUnexpectedTrailingTokenException(this, next);
					break;
			}
		}

		private void ParseTokenCodeBlock(Parser parser)
		{
			Token next = parser.NextToken;
			switch (StatementType)
			{
				case Type.If:
					if ((next is Punctuator pun && pun.PunctuatorType == Punctuator.Type.OpeningParentases && pun.Character == '{')
						|| (next is Operator op && op.OperatorType == Operator.Type.Assignment)
					    || (next is Statement))
						parser.TakeNextToken(_CodeBlock);
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