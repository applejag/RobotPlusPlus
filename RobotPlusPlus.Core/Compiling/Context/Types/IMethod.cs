using System.Reflection;
using JetBrains.Annotations;

namespace RobotPlusPlus.Core.Compiling.Context.Types
{
	public interface IMethod
	{
		[NotNull, ItemNotNull]
		MethodInfo[] MethodInfos { get; }
	}
}