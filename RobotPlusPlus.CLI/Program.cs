
using McMaster.Extensions.CommandLineUtils;

namespace RobotPlusPlus.CLI
{
	public class Program
	{
		public static int Main(string[] args)
			=> CommandLineApplication.Execute<ProgramOptions>(args);
	}
}
