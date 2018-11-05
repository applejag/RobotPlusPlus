using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Structures.G1ANT;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public class CSharpMethod : CSharpType, IMethod
	{
		public MethodInfo[] MethodInfos { get; }

		public CSharpMethod(
			[CanBeNull] Type baseType,
			[NotNull] MethodInfo[] methodInfos)
			: base(baseType, methodInfos[0].Name)
		{
			MethodInfos = methodInfos;
		}
	}
}