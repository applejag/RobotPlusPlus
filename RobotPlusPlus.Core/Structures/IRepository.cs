using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling.Context;
using RobotPlusPlus.Core.Compiling.Context.Types;

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

		void RegisterOther(ValueContext context);
	}
}