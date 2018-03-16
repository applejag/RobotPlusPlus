using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Exceptions;
using RobotPlusPlus.Parsing;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

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

			string initVerb = $"Reading from file \"{(options.Verbose ? sourceFile : Path.GetFileName(sourceFile))}\"";
			const string onErrorVerb = "reading from file";

			if (!TryExecAction(initVerb, onErrorVerb, () => File.ReadAllText(options.Script), out string content))
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

			if (!TryExecAction("Tokenizing code", "tokenization", () => Tokenizer.Tokenize(sourceCode),
				out tokenizedCode)) return false;

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

			return TryExecAction("Parsing code", "parsing", () => Parser.Parse(tokenizedCode), out parsedCode)
				   && TryExecAction("Compiling code", "compilation", () => Compiler.Compile(parsedCode), out compiledCode);
		}

		public bool WriteCompiledToDestination()
		{
			if (compiledCode == null)
				throw new InvalidOperationException("Code haven't been compiled yet!");

			string destination = options.Destination;

			if (!CheckIfDirectoryExists())
				return false;

			string folder = Path.GetDirectoryName(options.Destination);
			if (options.CreateFolder && Directory.Exists(folder))
			{
				if (!TryExecAction("Creating destination folder", "creating folder", () => Directory.CreateDirectory(folder).Exists, out bool success) || !success) {
					LogError("Failed creating destination folder, are you missing permission?");
					LogError($"\"{folder}\"");
					return false;
				} 
			}

			if (!TryExecAction("Writing compiled code to file", "writing to file",
				() => File.WriteAllText(destination, compiledCode)))
				return false;

			LogInfo("Success! The compiled code has been saved to:");
			console.ForegroundColor = ConsoleColor.Green;
			console.WriteLine();
			console.WriteLine(options.Destination);
			console.WriteLine();
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

		private bool TryExecAction(string initVerb, string onErrorVerb, Action action)
		{
			return TryExecAction(initVerb, onErrorVerb, () =>
			{
				action();
				return null;
			}, out object _);
		}

		private bool TryExecAction<TOut>(string initVerb,
			string onErrorVerb, Func<TOut> action, out TOut value)
		{
			Stopwatch watch = Stopwatch.StartNew();

			void ReportTime(ConsoleColor color, string status)
			{
				console.ResetColor();
				console.ForegroundColor = color;
				console.Write(status);
				watch.Stop();
				console.ForegroundColor = ConsoleColor.DarkGray;
				console.WriteLine($" (Took {watch.ElapsedMilliseconds}ms)");
				console.ResetColor();
			}

			try
			{
				console.ResetColor();
				console.ForegroundColor = ConsoleColor.Cyan;
				console.Write("[TASK] ");
				console.ForegroundColor = ConsoleColor.Gray;
				console.Write($"{initVerb}... ");
				console.ResetColor();

				value = action();

				ReportTime(ConsoleColor.Green, "Done.");
				return true;
			}
			catch (ParseException e)
			{
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
				ReportTime(ConsoleColor.Red, "Error!");
				Console.WriteLine();
				LogError($"Unexpected error during {onErrorVerb}!");
				Console.WriteLine();
				
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
				return false;
			}
#endif
		}

		public bool PreCompileWritingChecks()
		{
			if (!CheckIfDirectoryExists())
			{
				return false;
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