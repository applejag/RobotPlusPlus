
using System;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace RobotPlusPlus.CLI
{
	public class Program
	{
		public static int Main(string[] args)
		{

			try
			{
				return CommandLineApplication.Execute<ProgramOptions>(args);
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
