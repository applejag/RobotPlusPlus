using System;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class CSharpType : AbstractValue
	{
		[NotNull]
		public Type Type { get; }

		public CSharpType([NotNull] Type type)
			: this(type, type?.Name ?? "null", type?.Name ?? "null")
		{
		}

		public CSharpType([NotNull] Type type, [NotNull] string identifier)
			: this(type, identifier, identifier)
		{
		}

		public CSharpType([NotNull] Type type, [NotNull] string generated, [NotNull] string identifier)
			: base(generated, identifier)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
		}
	}
}