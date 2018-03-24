using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public abstract class CodeUnit
	{
		public FlexibleList<CodeUnit> PreUnits { get; }
		public FlexibleList<CodeUnit> PostUnits { get; }

		public Token Token { get; protected set; }

		public CodeUnit Parent { get; }
		public CodeUnit Root => Parent?.Root ?? this;
		public bool IsRoot => Root == this;

		protected CodeUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null)
		{
			Parent = parent;
			Token = token;
			PreUnits = parent?.PreUnits ?? new FlexibleList<CodeUnit>();
			PostUnits = parent?.PostUnits ?? new FlexibleList<CodeUnit>();
		}

		public abstract void Compile(Compiler compiler);
		public abstract string AssembleIntoString();

		public static CodeUnit CompileParsedToken([NotNull] Token token)
		{
			switch (token)
			{
				case OperatorToken op when op.OperatorType == OperatorToken.Type.Assignment:
					return new AssignmentUnit(op);

				case PunctuatorToken pun when pun.Character == ';':
					return null;

				default:
					throw new CompileUnexpectedTokenException(token);
			}
		}

	}
}