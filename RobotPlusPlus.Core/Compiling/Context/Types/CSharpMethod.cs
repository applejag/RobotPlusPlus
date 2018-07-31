using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class CSharpMethod : CSharpType
	{
		[NotNull, ItemNotNull]
		public MethodInfo[] MethodInfos { get; }

		public CSharpMethod(
			[CanBeNull] Type baseType,
			[NotNull] MethodInfo[] methodInfos)
			: base(baseType, methodInfos[0].Name)
		{
			MethodInfos = methodInfos;
		}

		[CanBeNull]
		public MethodInfo GetMethod(params Type[] types)
		{
			return MethodInfos
				.TryFirst(m => G1ANTMethodInfo.MethodMatches(m, types), out MethodInfo met) 
				? met : null;
		}
	}
}