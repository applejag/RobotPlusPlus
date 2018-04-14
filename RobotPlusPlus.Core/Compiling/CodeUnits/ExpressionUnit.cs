using System;
using System.Collections.Generic;
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

		public Type OutputType => throw new NotImplementedException();
		public bool NeedsCSSnippet { get; set; }

		private Dictionary<IdentifierToken, Variable> variableLookup;

		public ExpressionUnit([NotNull] Token token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
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

			// Check contains string and operator
			if (Token.AnyRecursive(t => t is LiteralStringToken, true)
				&& Token.AnyRecursive(t => t is OperatorToken, true))
				NeedsCSSnippet = true;

			// Check string needing escape chars
			if (Token.AnyRecursive(t => t is LiteralStringToken str
				&& str.Value.EscapeString() != str.Value, true))
				NeedsCSSnippet = true;

			variableLookup = new Dictionary<IdentifierToken, Variable>();

			// Loop variables
			Token.ForEachRecursive(t =>
			{
				if (!(t is IdentifierToken id)) return;

				variableLookup[id] = compiler.Context.FindVariable(id);

				if (id is IdentifierTempToken tmp
					&& string.IsNullOrEmpty(tmp.GeneratedName))
					throw new CompileException("Name not generated for temporary variable.", tmp);

				// Check variables for registration
				if (!compiler.Context.VariableExists(id))
					throw new CompileVariableUnassignedException(id);
			}, includeTop: true);
			
			foreach (CodeUnit post in PostUnits)
				post.Compile(compiler);
		}

		public override string AssembleIntoString()
		{
			return NeedsCSSnippet && !(Parent is IfUnit)
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

		#endregion

		#region Stringify expression tokens

		private string StringifyToken(Token token)
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
					return $"♥{variableLookup[id]}";

				case OperatorToken op when op.OperatorType == OperatorToken.Type.Assignment
					|| op.SourceCode == "++"
					|| op.SourceCode == "--":
					// Should've been extracted
					throw new CompileUnexpectedTokenException(token);

				case OperatorToken op when op.LHS != null && op.RHS != null:
					return $"{StringifyOperatorChildToken(op, op.LHS)}{op.SourceCode}{StringifyOperatorChildToken(op, op.RHS)}";

				case OperatorToken op when op.OperatorType == OperatorToken.Type.Unary:
					return $"{op.SourceCode}{StringifyOperatorChildToken(op, op.UnaryValue)}";

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