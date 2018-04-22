using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class ExpressionUnit : CodeUnit
	{
		public FlexibleList<CodeUnit> PreUnits { get; }
		public FlexibleList<CodeUnit> PostUnits { get; }

		public Type OutputType { get; private set; }
		public Type InputType { get; internal set; }
		public bool NeedsCSSnippet { get; set; }
		public UsageType Usage { get; }

		public Token ContainerToken { get; private set; }
		public Type ContainerType { get; private set; }

		public enum UsageType
		{
			Read,
			Write
		}

		public ExpressionUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null, UsageType usage = UsageType.Read)
			: base(token, parent)
		{
			Usage = usage;

			PreUnits = new FlexibleList<CodeUnit>();
			PostUnits = new FlexibleList<CodeUnit>();

			Token = RemoveParentases(token);
			Token = RemoveUnaries(Token);
			Token = ExtractInnerAssignments(Token);
		}

		public override void Compile(Compiler compiler)
		{
			NeedsCSSnippet = false;

			foreach (CodeUnit pre in PreUnits)
				pre.Compile(compiler);

			OutputType = EvalTokenType(Token, compiler, Usage, InputType, out bool needsCsSnippet);
			NeedsCSSnippet = needsCsSnippet;

			if (Usage == UsageType.Write)
			{
				ContainerToken = GetLeftmostToken(Token);
				ContainerType = EvalTokenReadType(ContainerToken, compiler);


				if (!TypeChecking.CanImplicitlyConvert(InputType, OutputType))
					throw new CompileTypeConvertImplicitAssignmentException(Token, InputType, OutputType);
			}

			foreach (CodeUnit post in PostUnits)
				post.Compile(compiler);
		}

		private static Token GetLeftmostToken(Token token)
		{
			while (true)
			{
				if (!(token is PunctuatorToken pun) || pun.PunctuatorType != PunctuatorToken.Type.Dot)
					return token;

				token = pun.DotLHS;
			}
		}

		public override string AssembleIntoString()
		{
			return NeedsCSSnippet && !(Parent is AbstractFlowUnit)
				? $"⊂{StringifyToken(Token)}⊃"
				: StringifyToken(Token);
		}

		#region Public utility

		public (ExpressionUnit, IdentifierTempToken) ExtractIntoTempAssignment()
		{
			(CodeUnit tempAssignment, IdentifierTempToken id)
				= AssignmentUnit.CreateTemporaryAssignment(Token, Parent);

			var exp = new ExpressionUnit(id, Parent);

			// Old preunits
			foreach (CodeUnit pre in PreUnits)
				exp.PreUnits.Add(pre);
			// Temp assignment
			exp.PreUnits.Add(tempAssignment);
			// Old postunits
			foreach (CodeUnit post in PostUnits)
				exp.PreUnits.Add(post);

			return (exp, id);
		}

		[CanBeNull, Pure]
		public static Type EvalTokenReadType([NotNull] Token token, [NotNull] Compiler compiler)
		{
			return EvalTokenType(token, compiler, UsageType.Read, null, out bool _);
		}

		[CanBeNull]
		public static Type EvalTokenWriteType([NotNull] Token token, [NotNull] Compiler compiler, [NotNull] Type inputType)
		{
			return EvalTokenType(token, compiler, UsageType.Write, inputType, out bool _);
		}

		[CanBeNull]
		private static Type EvalTokenType([NotNull] Token token, [NotNull] Compiler compiler, UsageType usage, Type inputType, out bool needsCSSnippet)
		{
			bool csSnippet = false, containsStr = false, containsOp = false;

			Type type = RecursiveCheck(token);

			needsCSSnippet = usage == UsageType.Read && (csSnippet || (containsStr && containsOp));
			return type;

			Type RecursiveCheck(Token t)
			{
				switch (t)
				{
					case IdentifierToken id:
						// Check variables for registration
						Variable variable = compiler.Context.FindVariable(id);

						if (variable == null)
						{
							if (usage == UsageType.Write)
								variable = compiler.Context.RegisterVariable(id, inputType);
							else
								throw new CompileVariableUnassignedException(id);
						}

						// Check generated name
						if (string.IsNullOrEmpty(id.GeneratedName))
						{
							if (id is IdentifierTempToken)
								throw new CompileException("Name not generated for temporary variable.", id);
							throw new CompileException($"Name not generated for variable <{id.Identifier}>.", id);
						}

						if (variable.Type == typeof(string))
							containsStr = true;

						return variable.Type;

					case LiteralNumberToken num:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(num);
						return num.Value.GetType();

					case LiteralStringToken str:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(str);
						containsStr = true;
						if (str.NeedsEscaping) csSnippet = true;
						return typeof(string);

					case LiteralKeywordToken key:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(key);
						return key.Value?.GetType();

					case PunctuatorToken pun when pun.PunctuatorType == PunctuatorToken.Type.Dot:
						string identifier = pun.DotRHS.Identifier;
						Type lhs = RecursiveCheck(pun.DotLHS)
						           ?? throw new CompileException($"Unvaluable property from dot LHS, <{pun.DotLHS}>.", pun.DotLHS);
						PropertyInfo property = lhs.GetProperty(identifier);

						if (property == null)
							throw new CompileTypePropertyDoesNotExistException(pun, lhs, identifier);
						if (usage == UsageType.Read && !property.CanRead)
							throw new CompileTypePropertyNoGetterException(pun, lhs, identifier);
						if (usage == UsageType.Write && !property.CanWrite)
							throw new CompileTypePropertyNoSetterException(pun, lhs, identifier);

						csSnippet = true;
						return property.PropertyType;

					case OperatorToken op when op.OperatorType == OperatorToken.Type.Unary:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(op);
						containsOp = true;
						return op.EvaluateType(RecursiveCheck(op.UnaryValue));

					case OperatorToken op:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(op);
						containsOp = true;
						Type lhsType = RecursiveCheck(op.LHS);
						Type rhsType = RecursiveCheck(op.RHS);
						return op.EvaluateType(lhsType, rhsType);
				}

				throw new CompileUnexpectedTokenException(t);
			}
		}

		#endregion

		#region Stringify expression tokens

		public string StringifyToken(Token token)
		{
			switch (token)
			{
				case LiteralStringToken str:
					return NeedsCSSnippet
						? $"\"{str.Value.EscapeString()}\""
						: $"‴{str.Value}‴";

				case LiteralNumberToken num:
					return num.AssembleIntoString();

				case LiteralKeywordToken _:
					return token.SourceCode;

				case IdentifierToken id:
					return $"♥{id.GeneratedName}";

				case OperatorToken op when op.OperatorType == OperatorToken.Type.Assignment
					|| op.SourceCode == "++"
					|| op.SourceCode == "--":
					// Should've been extracted
					throw new CompileUnexpectedTokenException(token);

				case OperatorToken op when op.LHS != null && op.RHS != null:
					return $"{StringifyOperatorChildToken(op, op.LHS)}{op.SourceCode}{StringifyOperatorChildToken(op, op.RHS)}";

				case OperatorToken op when op.OperatorType == OperatorToken.Type.Unary:
					return $"{op.SourceCode}{StringifyOperatorChildToken(op, op.UnaryValue)}";

				case PunctuatorToken pun when pun.PunctuatorType == PunctuatorToken.Type.Dot:
					return $"{StringifyToken(pun.DotLHS)}.{pun.DotRHS}";

				default:
					throw new CompileUnexpectedTokenException(token);
			}
		}

		private string StringifyOperatorChildToken(OperatorToken parent, Token child)
		{
			if (child is OperatorToken op && op.OperatorType > parent.OperatorType)
				return $"({StringifyToken(child)})";

			return StringifyToken(child);
		}

		#endregion

		#region Construction alterations

		private Token ExtractInnerAssignments(Token token, Token parent = null)
		{
			// Convert command call to assignment
			if (token is FunctionCallToken
			&& parent != null)
			{
				(CodeUnit unit, IdentifierTempToken temp) = AssignmentUnit.CreateTemporaryAssignment(token, this);
				PreUnits.Add(unit);
				token = temp;
			}

			// Convert prefix expressions
			if (token is OperatorToken pre && pre.OperatorType == OperatorToken.Type.PreExpression)
			{
				PreUnits.Add(CompileParsedToken(pre, this));
				token = pre.UnaryValue;
			}

			// Convert postfix expressions
			if (token is OperatorToken post && post.OperatorType == OperatorToken.Type.PostExpression)
			{
				PostUnits.Add(CompileParsedToken(post, this));
				token = post.UnaryValue;
			}

			// Convert assignment to expression
			if (token is OperatorToken op
				&& op.OperatorType == OperatorToken.Type.Assignment)
			{
				PreUnits.Add(CompileParsedToken(op, this));
				token = op.LHS;
			}

			// Run on childs
			for (var i = 0; i < token.Count; i++)
			{
				token[i] = ExtractInnerAssignments(token[i], token);
			}

			// Returns altered
			return token;
		}

		public static Token RemoveParentases(Token token, Token parent = null)
		{
			Repeat:

			if (token is PunctuatorToken pun
				&& pun.PunctuatorType == PunctuatorToken.Type.OpeningParentases
				&& pun.Character == '('
				&& !(parent is FunctionCallToken))
			{
				if (pun.Count != 1)
					throw new CompileIncorrectTokenCountException(1, pun);

				token = token[0];
				goto Repeat;
			}

			for (var i = 0; i < token.Count; i++)
			{
				token[i] = RemoveParentases(token[i], token);
			}

			return token;
		}

		public static Token RemoveUnaries(Token token, Token parent = null)
		{
			Repeat:

			if (token is OperatorToken op
				&& op.OperatorType == OperatorToken.Type.Unary)
			{
				// Remove + unary
				if (op.SourceCode == "+")
				{
					token = op.UnaryValue;
					goto Repeat;
				}

				// Remove double unary -(-x), !!x, ~~x
				if (op.UnaryValue is OperatorToken op2
					&& op.OperatorType == op2.OperatorType
					&& op.SourceCode == op2.SourceCode)
				{
					token = op2.UnaryValue;
					goto Repeat;
				}
			}

			for (var i = 0; i < token.Count; i++)
			{
				token[i] = RemoveUnaries(token[i], token);
			}

			return token;
		}

		#endregion

	}
}