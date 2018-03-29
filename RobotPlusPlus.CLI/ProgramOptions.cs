using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using RobotPlusPlus.CLI.Validation;

namespace RobotPlusPlus.CLI
{
	[VersionOptionFromMember(MemberName = nameof(Version))]
	[HelpOption]
	public class ProgramOptions
	{
		public string Version => "hellodog";

		#region Private option fields
		[Required]
		[FileExists]
		[FileExtensions(Extensions = ".robotpp")]
		[Argument(0, Name = "Script",
			Description = "Required. The `.robotpp` script to be parsed.")]
		private string OptScript { get; } = null;

		[ValidPathCharacters(ErrorMessage = "The output folder must not contains invalid characters!")]
		[Option("-d --dest <FOLDER>",
			Description = "The destination for the compiled `.robot` script.\n" +
						  "Default: Same folder as <Script>.")]
		private string OptDestinationFolder { get; } = null;

		[ValidFileNameCharacters(ErrorMessage = "The output filename must not contains invalid characters!")]
		[Option("-n --name <FILENAME>",
			Description = "The output filename. The extension `.robot` will be added if one is omitted.\n" +
						  "Default: Same name as <Script> but with `.robot` extension.")]
		private string OptDestinationFileName { get; } = null;

		[Option("-np --nopause",
			Description = "Disables the pause at the end of the compilation.")]
		private bool OptDontPauseAtEnd { get; } = false;

		[Option("-o --overwrite",
			Description = "Will overwrite without prompting the user.")]
		private bool OptOverwriteWithoutPrompt { get; set; } = false;

		[Option("-f --folder",
			Description = "Will create folder if it doesn't already exist.")]
		private bool OptCreateFolder { get; } = false;

		[Option("-q --quiet",
			Description = "Disables all writing to the console.\n" +
						  "This flag enables --Overwrite and --NoPause, and disables --Verbose.")]
		private bool OptQuietMode { get; } = false;

		[Option("-v --verbose",
			Description = "Show a more verbose output.")]
		private bool OptVerbose { get; } = false;

		[Option("-t --test",
			Description = "Only try to compile. Don't write anything to the destination.")]
		private bool OptDryRun { get; } = false;
		
		[Option("-ie --inencoding <NAME>",
			Description = "The encoding to use when reading the source file. Default: Current OS ASNI code page.")]
		private Encodings? OptInputEncoding { get; } = null;

		[Option("-oe --outencoding <NAME>",
			Description = "The encoding to use when writing the output file. Default: UTF-8.")]
		private Encodings? OptOutputEncoding { get; } = null;
		#endregion

		#region Public option fields

		public string Script => EvalSource();

		public string Destination => EvalDestination();

		public bool PauseAtEnd => !OptDontPauseAtEnd && !OptQuietMode;
		public bool Verbose => OptVerbose && !OptQuietMode;
		public bool QuietMode => OptQuietMode;
		public bool OverwriteWithoutPrompt
		{
			get => OptOverwriteWithoutPrompt || OptQuietMode;
			set => OptOverwriteWithoutPrompt = value;
		}

		public bool CreateFolder => OptCreateFolder;

		public bool DryRun => OptDryRun;

		public Encoding InputEncoding => OptInputEncoding.HasValue ? Encoding.GetEncoding((int)OptInputEncoding.Value) : Encoding.Default;
		public Encoding OutputEncoding => OptOutputEncoding.HasValue ? Encoding.GetEncoding((int)OptOutputEncoding.Value) : Encoding.UTF8;
		#endregion

		public static int Execute(string[] args)
		{
			return CommandLineApplication.Execute<ProgramOptions>(args);
		}

		[UsedImplicitly]
		private void OnExecute()
		{
			IConsole console = QuietMode ? NullConsole.Singleton : PhysicalConsole.Singleton;

#if !DEBUG
			try
			{
#endif
				var rw = new ReaderWriter(this, console);

				if (DryRun || rw.PreCompileWritingChecks())
				{

					bool success = rw.ReadCodeFromFile()
								   && rw.TokenizeCode()
								   && rw.CompileCode();

					if (success && !DryRun)
					{
						rw.WriteCompiledToDestination();
					}
				}
#if !DEBUG
			}
			catch (Exception e)
			{
	#if TRACE
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
	#endif
			}
			finally
			{
#endif
				if (PauseAtEnd)
				{
					// Clear buffer & colors
					while (Console.KeyAvailable) Console.ReadKey(true);
					Console.ResetColor();
					// Await input
					Console.Write("\nExecution finished. Press any key to exit...");
					Console.ReadKey(true);
					Console.WriteLine();
				}
#if !DEBUG
			}
#endif
		}

		private string EvalSource()
		{
			return Path.GetFullPath(OptScript);
		}

		private string EvalDestination()
		{
			string source = EvalSource();
			string filename = OptDestinationFileName;
			string folder = OptDestinationFolder;

			if (filename == null)
				filename = Path.ChangeExtension(Path.GetFileName(source), ".robot");

			if (folder == null)
				folder = Path.GetDirectoryName(source);

			return Path.Combine(folder, filename);
		}

		public enum Encodings
		{
			UTF7 = 65000,
			UTF8 = 65001,
			UTF16 = 1200,
			Windows1252 = 1252,
			UTF32 = 12000,
			UTF32BE = 12001,
			ASCII = 20127,
			ISO_8859_1 = 28591,
		}
	}
}