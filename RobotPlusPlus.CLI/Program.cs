using System;
using McMaster.Extensions.CommandLineUtils;

namespace RobotPlusPlus.CLI
{
	public class Program
	{
		public static int Main(string[] args)
		{
			try
			{
				return ProgramOptions.ExecuteAsync(args).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
#if DEBUG
				throw;
#else
				return -1;
#endif
			}
		}
	}
}
