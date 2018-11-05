using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Compiling.CodeUnits;
using RobotPlusPlus.Core.Compiling.Context.Types;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using RobotPlusPlus.Core.Utility;

namespace RobotPlusPlus.Core.Structures.G1ANT
{
    public class G1ANTMethodInfo : MethodInfo
    {
        [NotNull]
        public G1ANTRepository.CommandElement Command { get; }
        [CanBeNull]
        public G1ANTRepository.CommandFamilyElement CommandFamily { get; }
        [NotNull]
        public override Type ReflectedType => CommandFamily?.GetType() ?? throw new InvalidOperationException();

        [CanBeNull]
        public Type ResultType => CommandArguments
            .FirstOrDefault(c => c.ArgumentElement.Type == G1ANTRepository.Structure.Variable)
            ?.ArgumentElement.EvaluateVariableType()
            ?? typeof(void);

        public override Type DeclaringType => CommandFamily?.GetType();
        public override string Name => Command.Name;
        public G1ANTParameterInfo[] CommandArguments { get; }
        public G1ANTParameterInfo[] GlobalArguments { get; }

        private G1ANTMethodInfo(
            [NotNull] G1ANTRepository.CommandElement command,
            [CanBeNull, ItemNotNull] IEnumerable<G1ANTRepository.ArgumentElement> arguments,
            [NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
            [CanBeNull] G1ANTRepository.CommandFamilyElement family = null)
        {
            Command = command;
            CommandFamily = family;

            CommandArguments = arguments
                ?.Select((a, i) => new G1ANTParameterInfo(this, a, i)).ToArray()
                ?? new G1ANTParameterInfo[0];
            GlobalArguments = globalArguments.Arguments
                .Select((a, i) => new G1ANTParameterInfo(this, a, i + CommandArguments.Length)).ToArray();
        }

        public override ParameterInfo[] GetParameters()
        {
            return CommandArguments
                .Concat(GlobalArguments)
                .Cast<ParameterInfo>()
                .ToArray();
        }

        public G1ANTParameterInfo[] GetG1ANTParameters()
        {
            return CommandArguments
                .Concat(GlobalArguments)
                .ToArray();
        }

        internal static G1ANTMethodInfo[] ListMethods(
            [NotNull] G1ANTRepository.CommandElement command,
            [NotNull] G1ANTRepository.GlobalArgumentsElement globalArguments,
            [CanBeNull] G1ANTRepository.CommandFamilyElement family = null)
        {

            List<G1ANTRepository.ArgumentElement>[] overloads = command.Overloads?.Select(o => o.Arguments).ToArray()
                ?? new List<G1ANTRepository.ArgumentElement>[0];

            var info = new G1ANTMethodInfo[overloads.Length + 1];
            info[0] = new G1ANTMethodInfo(command, command.Arguments, globalArguments, family);

            for (var i = 0; i < overloads.Length; i++)
            {
                info[i + 1] = new G1ANTMethodInfo(command, overloads[i], globalArguments, family);
            }

            return info;
        }

        public static bool MethodMatches(FunctionCallToken token, MethodInfo method, CommandUnit.Argument[] args)
        {
            /**
			 * + Check for duplicate parameters
			 * + Check if all required parameters are used
			 * + Check if parameter types matches
			 * + Check if too many arguments, i.e. non-existing parameters
			 * + Check if named argument exists
			 */

            List<ParameterInfo> actuals = method.GetParameters().ToList();

            foreach (var duplicates in actuals
                .GroupBy(a => a.Name)
                .Where(g => g.Count() > 1))
            {
                throw new CompileParameterDuplicateException(method, duplicates.First(), token);
            }

            foreach (CommandUnit.Argument argument in args)
            {
                ParameterInfo param;

                if (argument is CommandUnit.NamedArgument named)
                {
                    // Lookup named
                    if (!actuals.TryFirst(p => p.Name == named.name, out param))
                        throw new CompileParameterNamedDoesntExistException(method, named.name, token);

                    // Assign index
                    named.index = param.Position;
                }
                else
                {
                    // Argument index exists?
                    if (!actuals.TryFirst(p => p.Position == argument.index, out param))
                        throw new CompileParameterIndexedDoesntExistException(method, argument.index, token);
                }

                // Correct type?
                var abstractValue = (CSharpType)argument.expression.OutputType;
                if (!TypeChecking.CanImplicitlyConvert(abstractValue.Type, param.ParameterType))
                    throw new CompileParameterTypeConvertImplicitException(method, abstractValue.Type, param, token);

                // Exclude from remaining list
                actuals.Remove(param);
            }

            foreach (ParameterInfo actual in actuals)
            {
                // Remaining one is required one?
                if (!actual.HasDefaultValue)
                {
                    throw new CompileParameterRequiredMissingException(method, actual, token);
                }
            }

            return true;
        }

        [CanBeNull]
        public static MethodInfo GetMethod(FunctionCallToken token, MethodInfo[] methodInfos, params CommandUnit.Argument[] args)
        {
            return methodInfos
                .TryFirst(m => MethodMatches(token, m, args), out MethodInfo met)
                ? met : null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return new object[0];
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new object[0];
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }

        public override MethodAttributes Attributes { get; } = MethodAttributes.Public | MethodAttributes.Static;
        public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
        public override MethodInfo GetBaseDefinition()
        {
            return this;
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();
    }
}