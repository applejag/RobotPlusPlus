using System;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class Variable : CSharpType
	{
		[NotNull]
		public IdentifierToken Token { get; }

		public bool IsTemporary => Token is IdentifierTempToken;
		public bool IsReadOnly { get; }
		public bool IsStaticType { get; }

		public Variable(
			[NotNull] string generated,
			[NotNull] IdentifierToken token,
			[CanBeNull] Type type,
			bool isReadOnly = false,
			bool isStaticType = false)
			: base(type, generated, token.Identifier)
		{
			Token = token ?? throw new ArgumentNullException(nameof(token));
			IsReadOnly = isReadOnly;
			IsStaticType = isStaticType;
		}
	}
}