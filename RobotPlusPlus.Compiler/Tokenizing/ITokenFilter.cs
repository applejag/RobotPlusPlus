using JetBrains.Annotations;

namespace RobotPlusPlus.Tokenizing
{
	public interface ITokenFilter
	{
		bool Evaluate([NotNull] string input);
	}
}