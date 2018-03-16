using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;

namespace RobotPlusPlus.CLI
{
	public class Program
	{
		public static int Main(string[] args)
		{
#if DEBUG
			return ProgramOptions.ExecuteAsync(args).GetAwaiter().GetResult();
#else
			try
			{
				return ProgramOptions.ExecuteAsync(args).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n[ UNEXPECTED EXCEPTION DURING EXECUTION! ]\n");
				Console.WriteLine(e);
				Console.ResetColor();
				return -1;
			}
#endif
		}
	}
}
