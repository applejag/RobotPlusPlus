using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.CodeUnits.ControlFlow;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Structures;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Tokenizing.Tokens.Literals;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class ExpressionUnit : CodeUnit
	{
		public FlexibleList<CodeUnit> PreUnits { get; }
		public FlexibleList<CodeUnit> PostUnits { get; }

		/// <summary>Value type from this expression</summary>
		public AbstractValue OutputType { get; private set; }
		/// <summary>Inbound type when this is LHS of an assignment</summary>
		public AbstractValue InputType { get; internal set; }
		public bool NeedsCSSnippet { get; set; }
		public UsageType Usage { get; internal set; }

		/// <summary>
		/// <para>Used upon assigning to structs and classes.</para>
		///
		/// Example: rect.width = 10
		/// where this="width"
		/// and Container="rect"
		/// </summary>
		public Token ContainerToken { get; private set; }
		public AbstractValue ContainerType { get; private set; }

		public Dictionary<IdentifierToken, Type> StaticVariables { get; private set; }
			= new Dictionary<IdentifierToken, Type>();

		public Dictionary<FunctionCallToken, CommandUnit> EmbeddedCommands { get; private set; }
			= new Dictionary<FunctionCallToken, CommandUnit>();

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

			token = RemoveParentases(token);
			token = RemoveUnaries(token);
			Token = ExtractInnerAssignments(token);
		}

		public override void Compile(Compiler compiler)
		{
			NeedsCSSnippet = false;

			// Keep track of commands
			EmbeddedCommands.Clear();
			// Extract function calls
			Token = ExtractInnerCommands(compiler, Token);

			foreach (CodeUnit pre in PreUnits)
				pre.Compile(compiler);


			OutputType = EvalTokenType(Token, compiler, EmbeddedCommands, Usage, InputType, out bool needsCsSnippet);
			NeedsCSSnippet = needsCsSnippet;

			// Keep track of static vars
			StaticVariables.Clear();
			Token.ForEachRecursive(token =>
			{
				if (token is IdentifierToken id)
				{
					var variable = compiler.Context.FindIdentifier(id) as Variable;
					if (variable?.IsStaticType == true)
						StaticVariables[id] = variable.Type;
				}
			}, true);

			if (Usage == UsageType.Write)
			{
				ContainerToken = GetLeftmostToken(Token);
				ContainerType = EvalTokenReadType(ContainerToken, compiler, EmbeddedCommands);

				Type inputType = (InputType as CSharpType)?.Type ?? InputType.GetType();
				Type outputType = (OutputType as CSharpType)?.Type ?? OutputType.GetType();

				if (!TypeChecking.CanImplicitlyConvert(inputType, outputType))
					throw new CompileTypeConvertImplicitAssignmentException(Token, inputType, outputType);
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
			return AssembleIntoString(NeedsCSSnippet);
		}

		public string AssembleIntoString(bool withCSSnippetChars)
		{
			return withCSSnippetChars
				? $"⊂{StringifyToken(Token)}⊃"
				: StringifyToken(Token);
		}

        #region Public utility

	    public CommandUnit GetAsG1ANTCommandUnit()
	    {
	        if (!(Token is FunctionCallToken func)) return null;
	        CommandUnit cmd = EmbeddedCommands.GetValueOrDefault(func);
	        return cmd.MethodInfo is G1ANTMethodInfo ? cmd : null;
	    }

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
		public static AbstractValue EvalTokenReadType([NotNull] Token token, [NotNull] Compiler compiler, [NotNull] Dictionary<FunctionCallToken, CommandUnit> commandLookup)
		{
			return EvalTokenType(token, compiler, commandLookup, UsageType.Read, null, out bool _);
		}

		[CanBeNull]
		public static AbstractValue EvalTokenWriteType([NotNull] Token token, [NotNull] Compiler compiler, [NotNull] Dictionary<FunctionCallToken, CommandUnit> commandLookup, [NotNull] AbstractValue inputType)
		{
			return EvalTokenType(token, compiler, commandLookup, UsageType.Write, inputType, out bool _);
		}

		[NotNull]
		private static AbstractValue EvalTokenType([NotNull] Token token, [NotNull] Compiler compiler, [NotNull] Dictionary<FunctionCallToken, CommandUnit> commandLookup, UsageType usage, AbstractValue inputType, out bool needsCSSnippet)
		{
			bool csSnippet = false, containsStr = false, containsOp = false;

			AbstractValue type = RecursiveCheck(token);

			needsCSSnippet = usage == UsageType.Read && (csSnippet || (containsStr && containsOp));
			return type;

			AbstractValue RecursiveCheck(Token t)
			{
				switch (t)
				{
					case FunctionCallToken func:
                        CommandUnit cmdUnit = commandLookup[func];
					    if (cmdUnit.Method.NeedsCSSnippet)
					        csSnippet = true;
					    Type returnType = cmdUnit.MethodInfo.GetValueType();
					    if (returnType == null)
					    {
                            throw new CompileFunctionValueOfVoidException(cmdUnit.MethodInfo, cmdUnit.Token);
					    }
					    return new CSharpType(returnType);

				    case IdentifierToken id:
						// Check variables for registration
						AbstractValue value = compiler.Context.FindIdentifier(id);

						if (value is null)
						{
							if (usage == UsageType.Write)
								value = compiler.Context.RegisterVariable(id, (inputType as CSharpType)?.Type);
							else
								throw new CompileVariableUnassignedException(id);
						}

						if (value is Variable variable)
						{
							if (usage == UsageType.Write && variable.IsReadOnly)
								throw new CompileTypeReadOnlyException(variable.Token);

							if (variable.IsStaticType)
								csSnippet = true;

							// Check generated name
							if (string.IsNullOrEmpty(id.GeneratedName))
							{
								if (id is IdentifierTempToken)
									throw new CompileException("Name not generated for temporary variable.", id);
								throw new CompileException($"Name not generated for variable <{id.Identifier}>.", id);
							}

							if (variable.Type == typeof(string))
								containsStr = true;
						}

						return value;

					case LiteralNumberToken num:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(num);
						return new CSharpType(num.Value.GetType());

					case LiteralStringToken str:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(str);
						containsStr = true;
						if (str.NeedsEscaping) csSnippet = true;
						return new CSharpType(typeof(string));

					case LiteralKeywordToken key:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(key);
						return new CSharpType(key.Value?.GetType());

					/**
					 * Dot operation, ex: x.y, "string".ToUpper
					 */
					case PunctuatorToken pun when pun.PunctuatorType == PunctuatorToken.Type.Dot:
						string identifier = pun.DotRHS.Identifier;
						AbstractValue lhs = RecursiveCheck(pun.DotLHS)
											?? throw new CompileException(
												$"Unvaluable property from dot LHS, <{pun.DotLHS}>.", pun.DotLHS);

						if (lhs is CSharpType lhsCS)
						{
							if (lhsCS.Type == null)
								return lhsCS;

						    csSnippet = true;

                            BindingFlags flags = BindingFlags.Instance
												 | BindingFlags.Public
												 | BindingFlags.FlattenHierarchy;

							// If base is static variable
							if (lhsCS is Variable lhsVar && lhsVar.IsStaticType)
							{
								// Change to search for static fields
								flags &= ~BindingFlags.Instance;
								flags |= BindingFlags.Static;
							}

							// Do the search
							MemberInfo[] memberInfos = lhsCS.Type.GetMember(identifier, flags);

							// Validate
							if (memberInfos.Length == 0)
								throw new CompileTypePropertyDoesNotExistException(pun, lhsCS.Type, identifier);

							if (memberInfos[0] is MethodInfo)
							{
								// Method
								MethodInfo[] methodInfos = memberInfos.OfType<MethodInfo>().ToArray();

								if (methodInfos.Length != memberInfos.Length)
									throw new CompileTypePropertyException($"Disambigous property identification, <{memberInfos.Length}> options found for <{identifier}> on <{lhsCS.Type}>.", pun, lhsCS.Type, identifier);

								return new CSharpMethod(methodInfos[0].DeclaringType, methodInfos);
							}

							// Property or field
							MemberInfo memberInfo = memberInfos[0];
							if (usage == UsageType.Read && !memberInfo.CanRead())
								throw new CompileTypePropertyNoGetterException(pun, lhsCS.Type, identifier);
							if (usage == UsageType.Write && !memberInfo.CanWrite())
								throw new CompileTypePropertyNoSetterException(pun, lhsCS.Type, identifier);

							return new CSharpType(memberInfo.GetValueType());
						}
						else if (lhs is G1ANTFamily lhsFam)
						{
							if (!lhsFam.Commands.TryFirst(c => c.Identifier == identifier, out G1ANTCommand cmd))
								throw new CompileTypePropertyDoesNotExistException(pun, lhsFam.GetType(),
									$"{lhsFam.Identifier}.{identifier}");

							return cmd;
						}
						else if (lhs is G1ANTCommand lhsCmd)
						{
							throw new CompileTypePropertyDoesNotExistException(pun, lhsCmd.GetType(),
								$"{lhsCmd.Identifier}.{identifier}");
						}

						throw new InvalidOperationException($"Unknown type, <{lhs.GetType()}>");

					/**
					 * Unary evaluation, ex: -x, -5
					 */
					case OperatorToken op when op.OperatorType == OperatorToken.Type.Unary:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(op);
						containsOp = true;

						AbstractValue valType = RecursiveCheck(op.UnaryValue);
						if (valType is CSharpType valCSharp)
							return new CSharpType(op.EvaluateType(valCSharp.Type));

						throw new CompileTypeInvalidOperationException(op, valType.GetType());

					/**
					 * Twosided operation, ex: 1+2, x*3, "hi"+5
					 */
					case OperatorToken op:
						if (usage == UsageType.Write)
							throw new CompileExpressionCannotAssignException(op);
						containsOp = true;
						AbstractValue lhsType = RecursiveCheck(op.LHS);
						AbstractValue rhsType = RecursiveCheck(op.RHS);

						if (lhsType is CSharpType lhsCSharp
						&& rhsType is CSharpType rhsCSharp)
							return new CSharpType(op.EvaluateType(lhsCSharp.Type, rhsCSharp.Type));

						throw new CompileTypeInvalidOperationException(op, lhsType.GetType(), rhsType.GetType());
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
					return StaticVariables.TryGetValue(id, out Type type)
						? type.FullName
						: $"♥{id.GeneratedName}";

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

				case FunctionCallToken func:
					CommandUnit cmd = EmbeddedCommands[func];
					if (cmd.MethodInfo is G1ANTMethodInfo)
						throw new CompileException("G1ANT command wasen't extracted!", func);

					return cmd.AssembleIntoString();

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

		private Token ExtractInnerCommands(Compiler compiler, Token token, Token parent = null)
		{
			// Convert command call to assignment
			if (token is FunctionCallToken func
			&& !EmbeddedCommands.ContainsKey(func))
			{
				// Add it to list
				var cmd = new CommandUnit(func, this);
				EmbeddedCommands[func] = cmd;
				cmd.Compile(compiler);

				// Only extract g1ant methods
				if (cmd.MethodInfo is G1ANTMethodInfo && parent != null)
				{
				    (CodeUnit unit, IdentifierTempToken temp) =
				        AssignmentUnit.CreateTemporaryAssignment(token, this);
				    PreUnits.Add(unit);
				    token = temp;
				}
			}

			// Run on childs
			for (var i = 0; i < token.Count; i++)
			{
				token[i] = ExtractInnerCommands(compiler, token[i], token);
			}

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