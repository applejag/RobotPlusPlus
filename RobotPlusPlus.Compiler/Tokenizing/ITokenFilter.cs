using JetBrains.Annotations;

namespace RobotPlusPlus.Tokenizing
{
	public interface ITokenFilter
	{
		int MatchingLength([NotNull] string input);
	}
}