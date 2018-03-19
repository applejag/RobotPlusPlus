using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using RobotPlusPlus.Core.Compiling;
using RobotPlusPlus.Core.Exceptions;
using RobotPlusPlus.Core.Parsing;
using RobotPlusPlus.Core.Tokenizing;
using RobotPlusPlus.Core.Tokenizing.Tokens;

namespace RobotPlusPlus.CLI
{
	public class ReaderWriter
	{
		private readonly ProgramOptions options;

		private Token[] tokenizedCode;
		private Token[] parsedCode;
		private string compiledCode;
		private string sourceCode;
		private string sourceFile;
		private readonly IConsole console;

		public ReaderWriter(ProgramOptions options, IConsole console)
		{
			this.options = options;
			this.console = console;
		}

		public bool ReadCodeFromFile()
		{
			sourceFile = options.Script;

			if (!File.Exists(sourceFile))
				throw new FileNotFoundException("Script file was not found!", sourceFile);

			string encoding = options.Verbose ? $" using encoding {options.InputEncoding.EncodingName}" : string.Empty;
			string initVerb = $"Reading from \"{Path.GetFileName(sourceFile)}\"{encoding}";
			const string onErrorVerb = "reading from file";

			if (!TryExecAction(initVerb, onErrorVerb, () => File.ReadAllText(options.Script, options.InputEncoding), out string content))
				return false;

			sourceCode = ReplaceNewLines(content);
			return true;

		}

		public static string ReplaceNewLines(string multiline)
		{
			return multiline.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n");
		}

		/// <exception cref="ParseException"></exception>
		public bool TokenizeCode()
		{
			if (sourceCode == null)
				throw new InvalidOperationException("Code haven't been read yet!");

			string file = Path.GetFileName(sourceFile);
			if (!TryExecAction("Tokenizing code", "tokenization", () => Tokenizer.Tokenize(sourceCode, file),
				out tokenizedCode, log: options.Verbose)) return false;

			if (options.Verbose)
			{
				LogInfo("Colorized tokenized code:");
				PrettyConsoleWriter.WriteCodeToConsole(tokenizedCode, console);
			}

			return true;
		}

		/// <exception cref="ParseException"></exception>
		public bool CompileCode()
		{
			if (tokenizedCode == null)
				throw new InvalidOperationException("Code haven't been tokenized yet!");

			return TryExecAction("Parsing code", "parsing", () => Parser.Parse(tokenizedCode), out parsedCode, log: options.Verbose)
				   && TryExecAction("Compiling code", "compilation", () => Compiler.Compile(parsedCode), out compiledCode);
		}

		public bool WriteCompiledToDestination()
		{
			if (compiledCode == null)
				throw new InvalidOperationException("Code haven't been compiled yet!");

			string destination = options.Destination;

			if (!CheckIfDirectoryExists())
				return false;
			if (!CheckIfDestinationFileExists())
				return false;

			string folder = Path.GetDirectoryName(options.Destination);
			if (options.CreateFolder && Directory.Exists(folder))
			{
				if (!TryExecAction("Creating destination folder", "creating destination folder",
					    () => Directory.CreateDirectory(folder).Exists, out bool success, log: options.Verbose) || !success)
				{
					LogError("Failed creating destination folder, are you missing permission?");
					LogError($"\"{folder}\"");
					return false;
				}
			}

			string filename = Path.GetFileName(options.Destination);
			string encoding = options.Verbose ? $" using encoding {options.OutputEncoding.EncodingName}" : string.Empty;
			if (!TryExecAction($"Writing file \"{filename}\"{encoding}", "writing to file",
				() => File.WriteAllText(destination, compiledCode, options.OutputEncoding)))
				return false;

			console.WriteLine();
			LogInfo("Success! The compiled code has been saved to:");
			console.ForegroundColor = ConsoleColor.Green;
			console.WriteLine(options.Destination);
			console.ResetColor();

			return true;
		}

		private void LogInfo(string info)
		{
			console.ResetColor();
			console.ForegroundColor = ConsoleColor.Yellow;
			console.Write("[INFO] ");
			console.ForegroundColor = ConsoleColor.Gray;
			console.WriteLine(info);
			console.ResetColor();
		}

		private void LogError(string error)
		{
			console.ResetColor();
			console.ForegroundColor = ConsoleColor.Red;
			console.Write("[ERROR] ");
			console.ForegroundColor = ConsoleColor.White;
			console.WriteLine(error);
			console.ResetColor();
		}

		private bool TryExecAction(string initVerb, string onErrorVerb, Action action, bool log = true)
		{
			return TryExecAction(initVerb, onErrorVerb, () =>
			{
				action();
				return null;
			}, out object _, log);
		}

		private bool TryExecAction<TOut>(string initVerb,
			string onErrorVerb, Func<TOut> action, out TOut value, bool log = true)
		{
			Stopwatch watch = log ? Stopwatch.StartNew() : null;

			void ReportTime(ConsoleColor color, string status)
			{
				console.ResetColor();
				console.ForegroundColor = color;
				console.Write(status);

				if (options.Verbose)
				{
					watch.Stop();
					console.ForegroundColor = ConsoleColor.DarkGray;
					console.WriteLine($" (Took {watch.ElapsedMilliseconds}ms)");
					console.ResetColor();
				}
				else
					console.WriteLine();
			}

			try
			{
				if (log)
				{
					console.ResetColor();
					console.ForegroundColor = ConsoleColor.Cyan;
					console.Write("[TASK] ");
					console.ForegroundColor = ConsoleColor.Gray;
					console.Write($"{initVerb}... ");
					console.ResetColor();
				}

				value = action();

				if (log)
					ReportTime(ConsoleColor.Green, "Done.");
				return true;
			}
			catch (ParseException e)
			{
				if (log)
					ReportTime(ConsoleColor.Red, "Error!");
				LogError($"Error while {onErrorVerb}!");

				console.WriteLine();
				LogError($"{(options.Verbose ? sourceFile : Path.GetFileName(sourceFile))}:{e.Line}: {e.Message}");

				PrettyConsoleWriter.WriteCodeHighlightError(sourceCode, e, options.Verbose ? -1 : 12, console);

				console.ResetColor();
				value = default;
				return false;
			}
#if !DEBUG
			catch (Exception e)
			{
				if (log) {
					ReportTime(ConsoleColor.Red, "Error!");
					Console.WriteLine();
				}
				LogError($"Unexpected error during {onErrorVerb}!");
				Console.WriteLine();
				
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
				value = default;
				return false;
			}
#endif
		}

		public bool PreCompileWritingChecks()
		{
			if (!CheckIfDirectoryExists())
				return false;

			if (!CheckIfDestinationFileExists())
				return false;

			return true;
		}

		private bool CheckIfDestinationFileExists()
		{
			string destination = options.Destination;
			if (File.Exists(destination) && !options.OverwriteWithoutPrompt)
			{
				if (!Prompt.GetYesNo(
					$"The destination file \"{Path.GetFileName(destination)}\" already exists, do you wish to override it?", true))
					return false;

				options.OverwriteWithoutPrompt = true;
			}

			return true;
		}

		private bool CheckIfDirectoryExists()
		{
			string folder = Path.GetDirectoryName(options.Destination);
			if (!options.CreateFolder && !Directory.Exists(folder))
			{
				LogError("Target directory doesn't exist:");
				LogError($"\"{folder}\"");
				LogError("Use the --createfolder flag to automatically create the destination folder.");
				return false;
			}

			return true;
		}
	}
}