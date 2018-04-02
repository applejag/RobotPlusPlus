using System;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using RobotPlusPlus.Core.Compiling;

namespace RobotPlusPlus.CLI
{
	public class Program
	{

		public static string CompilerVersion { get; } = FormatVersion(Assembly.GetAssembly(typeof(Compiler)).GetName().Version);

		public static string CLIVersion { get; } = FormatVersion(Assembly.GetExecutingAssembly().GetName().Version);

		private static string FormatVersion(Version v)
		{
			return v.Minor % 2 == 0 ? v.ToString(3) : $"{v}-indev";
		}

		public static string GetLongVersion()
		{
			return $"RobotPlusPlus.Core v{CompilerVersion}\r\nRobotPlusPlus.CLI v{CLIVersion}";
		}

		public static string GetShortVersion()
		{
			return $"Core v{CompilerVersion}, CLI v{CLIVersion}";
		}

		public static int Main(string[] args)
		{
			var app = new CommandLineApplication<ProgramOptions>
			{
				FullName = "G1ANT.Robot++ Compiler CLI",
				Name = "RobotPlusPlus.CLI",
			};
			app.Conventions.UseDefaultConventions();
			app.VersionOption("-v --version", GetShortVersion, GetLongVersion);

#if DEBUG
			return app.Execute(args);
#else
			try
			{
				return app.Execute(args);
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
