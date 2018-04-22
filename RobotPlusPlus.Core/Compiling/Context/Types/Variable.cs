using System;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class Variable : AbstractValue
	{
		[NotNull]
		public Type Type { get; }

		[NotNull]
		public IdentifierToken Token { get; }

		public bool IsTemporary => Token is IdentifierTempToken;
		public bool IsReadOnly { get; }
		public bool IsStaticType { get; }

		public Variable(
			[NotNull] string generated,
			[NotNull] IdentifierToken token,
			[NotNull] Type type,
			bool isReadOnly = false,
			bool isStaticType = false)
			: base(generated, token.Identifier)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			Token = token ?? throw new ArgumentNullException(nameof(token));
			IsReadOnly = isReadOnly;
			IsStaticType = isStaticType;
		}
	}
}