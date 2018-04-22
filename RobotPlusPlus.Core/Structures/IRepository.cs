using System;
using System.Collections.Generic;
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
	}
}