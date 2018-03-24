using System;
using JetBrains.Annotations;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.Core.Compiling.CodeUnits
{
	public class AssignmentUnit : CodeUnit
	{
		public ExpressionUnit Expression { get; }
		public VariableUnit AssignedVariable { get; private set; }

		public AssignmentUnit([NotNull] OperatorToken token, [CanBeNull] CodeUnit parent = null)
			: base(token, parent)
		{
			if (token.OperatorType != OperatorToken.Type.Assignment)
				throw new CompileUnexpectedTokenException(token);
			
			Expression = new ExpressionUnit(token.RHS, this);
		}

		public override void Compile(Compiler compiler)
		{
			throw new NotImplementedException();
		}

		public override string AssembleIntoString()
		{
			throw new NotImplementedException();
		}
	}
}