using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public abstract class CodeUnit
	{
		public Token Token { get; protected set; }

		public CodeUnit Parent { get; }
		public CodeUnit Root => Parent?.Root ?? this;
		public bool IsRoot => Parent == null;

		protected CodeUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null)
		{
			Parent = parent;
			Token = token;
		}

		public virtual void PreCompile(Compiler compiler) { }
		public virtual void PostCompile(Compiler compiler) { }
		public abstract void Compile(Compiler compiler);
		public abstract string AssembleIntoString();

		public static CodeUnit CompileParsedToken([NotNull] Token token)
		{
			return CompileParsedToken(token, null);
		}

		public static CodeUnit CompileParsedToken([NotNull] Token token, [CanBeNull] CodeUnit parent)
		{
			switch (token)
			{
				case OperatorToken op when op.OperatorType == OperatorToken.Type.Assignment:
					if (!(op.LHS is IdentifierToken lhs))
						throw new CompileUnexpectedTokenException(op.LHS);
					if (op.RHS is FunctionCallToken rhs)
						return new CommandUnit(rhs, lhs, parent);

					return new AssignmentUnit(op, parent);

				case OperatorToken op when op.OperatorType == OperatorToken.Type.PreExpression
					|| op.OperatorType == OperatorToken.Type.PostExpression:

					return new AssignmentUnit(OperatorToken.ConvertPostPrefixToAssignment(op), parent);

				case PunctuatorToken pun when pun.Character == ';':
					return null;

				case PunctuatorToken pun when pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases
					&& pun.Character == '{':
					return new CodeBlockUnit(token, parent);

				case StatementToken st when st.StatementType == StatementToken.Type.If:
					return new IfUnit(st, parent);

				case FunctionCallToken func:
					return new CommandUnit(func, null, parent);

				default:
					throw new CompileUnexpectedTokenException(token);
			}
		}

	}
}