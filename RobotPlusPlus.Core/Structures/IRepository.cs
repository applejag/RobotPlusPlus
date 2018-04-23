using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Structures
{
	public interface IRepository
	{
		[NotNull]
		IEnumerable<(string id, Type type)> RegisterVariables();
		[NotNull]
		IEnumerable<(string id, Type type)> RegisterReadOnlyVariables();
		[NotNull]
		IEnumerable<(string id, Type type)> RegisterStaticTypes();

		[CanBeNull]
		MethodInfo LookupMethodInfo([CanBeNull] string family, [NotNull] string method);
	}
}