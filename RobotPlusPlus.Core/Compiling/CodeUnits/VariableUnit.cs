using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Tokenizing.Tokens;
using ValueType = RobotPlusPlus.Core.Structures.ValueType;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class VariableUnit : CodeUnit
	{
		public ValueType ValueType = ValueType.Null;
		public string GeneratedName { get; private set; }

		public VariableUnit([NotNull] Token token) : base(token, null)
		{ }

		public override void Compile(Compiler compiler)
		{
			throw new NotImplementedException();
		}

		public override string AssembleIntoString()
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			return obj is VariableUnit var &&
				   GeneratedName == var.GeneratedName;
		}

		public override int GetHashCode()
		{
			return 906031601 + EqualityComparer<string>.Default.GetHashCode(GeneratedName);
		}
	}
}