using System;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class ExpressionUnit : CodeUnit
	{
		public ExpressionUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			Token = RemoveParentases(token);
			Token = ExtractInnerAssignments(Token);
		}

		public override void Compile(Compiler compiler)
		{
			throw new NotImplementedException();
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
				token = op.RHS;
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