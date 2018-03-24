using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class ExpressionUnit : CodeUnit
	{
		public bool NeedsCSSnippet { get; set; }

		public ExpressionUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			Token = RemoveParentases(token);
			Token = ExtractInnerAssignments(Token);
		}

		public override void Compile(Compiler compiler)
		{
			// Check variables for registration
			if (Token.TryFirstRecursive(t => t is IdentifierToken
				&& !compiler.VariableContext.PrefferedExists(t.SourceCode), out Token var, true))
				throw new UnassignedVariableException(var);

			// Check contains mix of string n number
			if (Token.AnyRecursive(t => t is LiteralStringToken, true)
			    && Token.AnyRecursive(t => t is LiteralNumberToken, true))
				NeedsCSSnippet = true;
		}

		public override string AssembleIntoString()
		{
			throw new NotImplementedException();
		}

		#region Construction alterations

		private Token ExtractInnerAssignments(Token token)
		{
			// TODO: Add support for x++, x--, ++x, --x, ?:
			if (token is OperatorToken op
			    && op.OperatorType == OperatorToken.Type.Assignment)
			{
				PreUnits.Add(new AssignmentUnit(op, this));
				token = op.LHS;
			}

			for (var i = 0; i < token.Count; i++)
			{
				token[i] = ExtractInnerAssignments(token[i]);
			}

			return token;
		}

		public static Token RemoveParentases(Token token)
		{
		Repeat:

			if (token is PunctuatorToken pun
				&& pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases
				&& pun.Character == '(')
			{
				if (pun.Count != 1)
					throw new CompileIncorrectTokenCountException(1, pun);

				token = token[0];
				goto Repeat;
			}

			for (var i = 0; i < token.Count; i++)
			{
				token[i] = RemoveParentases(token[i]);
			}

			return token;
		}

		#endregion
	}
}