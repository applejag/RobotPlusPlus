
using McMaster.Extensions.CommandLineUtils;

namespace RobotPlusPlus.Console
{
	public class Program
	{

		public static void Main(string[] args)
		{
			CommandLineApplication.Execute<ProgramOptions>(args);
		}
	}
}
