using System;
using System.Linq;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.Context
{
	public static class ContextExtensions
	{
		#region Variables

		[NotNull, Pure]
		public static string GenerateVariableName([NotNull] this ValueContext context,
			[NotNull] IdentifierToken token)
		{
			return token is IdentifierTempToken
				? context.GenerateName("tmp")
				: context.GenerateName(token.Identifier);
		}

		[NotNull]
		public static Variable RegisterVariable([NotNull] this ValueContext context,
			[NotNull] IdentifierToken token, [NotNull] Type type)
		{
			var value = new Variable(GenerateVariableName(context, token), token, type);

			return token is IdentifierTempToken
				? context.RegisterValueDecayed(value)
				: context.RegisterValue(value);
		}

		[NotNull]
		public static Variable RegisterVariableGlobally([NotNull] this ValueContext context,
			[NotNull] IdentifierToken token, [NotNull] Type type)
		{
			var value = new Variable(GenerateVariableName(context, token), token, type);

			return token is IdentifierTempToken
				? context.RegisterValueDecayed(value)
				: context.RegisterValueGlobally(value);
		}

		[CanBeNull, Pure]
		public static Variable FindVariable([NotNull] this ValueContext context,
			[NotNull] IdentifierToken token)
		{
			return token is IdentifierTempToken
				? context.DecayedValues.OfType<Variable>().FirstOrDefault(x => x.Token == token)
				: context.FindValue(token.Identifier) as Variable;
		}

		[Pure]
		public static bool VariableExists([NotNull] this ValueContext context,
			[NotNull] IdentifierToken token)
		{
			return FindVariable(context, token) != null;
		}

		#endregion

		#region Labels

		[NotNull]
		public static Label RegisterLabelDecayed([NotNull] this ValueContext context,
			[NotNull] string label)
		{
			var value = new Label(context.GenerateName(label));

			return context.RegisterValueDecayed(value);
		}

		#endregion
	}
}