using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using McMaster.Extensions.CommandLineUtils;

namespace RobotPlusPlus.CLI
{
	[HelpOption]
	public class ProgramOptions
	{
		[Required]
		[FileExists]
		[Argument(0, Name = "Script",
			Description = "Required. The `.robotpp` script to be parsed.")]
		public string Script { get; }

		[Option(ShortName = "d", LongName = "Destination", ValueName = "Folder",
			Description = "The destination for the compiled `.robot` script. " +
			              "If none is supplied, it will be same folder and filename " +
			              "as the `.robotpp` script (with `.robot` extension).")]
		public string Destination { get; }

		private void OnExecute()
		{
			string dest = Path.Combine(Path.GetDirectoryName(Script), Destination ?? "", Path.ChangeExtension(Path.GetFileName(Script), ".robot"));

			Console.WriteLine($"I should use the file \"{Script}\" apprently, and output to \"{dest}\".");
			Console.ReadLine();
		}
	}
}