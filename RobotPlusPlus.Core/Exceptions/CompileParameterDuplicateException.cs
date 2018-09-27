using System;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Exceptions
{
    public class CompileParameterDuplicateException : CompileParameterException
    {
        public int ParameterIndex { get; }
        [NotNull]
        public string ParameterName { get; }

        public CompileParameterDuplicateException(
            [NotNull] MethodInfo method,
            [NotNull] ParameterInfo param,
            [NotNull] Token source)
            : this(method, param, source, null)
        {
        }

        public CompileParameterDuplicateException(
            [NotNull] MethodInfo method,
            [NotNull] ParameterInfo param,
            [NotNull] Token source,
            [CanBeNull] Exception innerException)
            : base($"Method <{method.Name}> has parameter <{param.Name}> assigned multiple times.",
                method, source, innerException)
        {
            ParameterIndex = param.Position;
            ParameterName = param.Name;
        }
    }
}