using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using RobotPlusPlus.Compiling;
using RobotPlusPlus.Parsing;
using RobotPlusPlus.Tokenizing;
using RobotPlusPlus.Tokenizing.Tokens;

namespace RobotPlusPlus.CLI
{
	public class ReaderWriter
	{
		private readonly ProgramOptions options;

		private Token[] tokenizedCode;
		private string compiledCode;
		private string sourceCode;
		private string sourceFile;

		public ReaderWriter(ProgramOptions options)
		{
			this.options = options;
		}

		public async Task ReadCodeFromFile(IConsole console)
		{
			sourceFile = options.Script;

			if (!File.Exists(sourceFile))
				throw new FileNotFoundException("Script file was not found!", sourceFile);

			try
			{
				console.ResetColor();
				console.Write($"Reading from file \"{(options.Verbose ? sourceFile : Path.GetFileName(sourceFile))}\"... ");
				sourceCode = ReplaceNewLines(await File.ReadAllTextAsync(options.Script));
				console.ForegroundColor = ConsoleColor.Green;
				console.WriteLine("Done.");
			}
			catch
			{
				console.ForegroundColor = ConsoleColor.Red;
				console.WriteLine("Error!");
				throw;
			}
		}

		public static string ReplaceNewLines(string multiline)
		{
			return multiline.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n");
		}

		/// <exception cref="ParseException"></exception>
		public void TokenizeCode(IConsole console)
		{
			if (sourceCode == null)
				throw new InvalidOperationException("Code haven't been read yet!");

			try
			{
				console.ResetColor();
				console.Write("Tokenizing code... ");
				tokenizedCode = Tokenizer.Tokenize(sourceCode);
				console.ForegroundColor = ConsoleColor.Green;
				console.WriteLine("Done.");
			}
			catch
			{
				console.ForegroundColor = ConsoleColor.Red;
				console.WriteLine("Error!");
				throw;
			}

			if (options.Verbose)
				PrettyConsoleWriter.WriteCodeToConsole(tokenizedCode, console);
		}

		/// <exception cref="ParseException"></exception>
		public void CompileCode(IConsole console)
		{
			if (tokenizedCode == null)
				throw new InvalidOperationException("Code haven't been tokenized yet!");

			try
			{
				console.ResetColor();
				console.Write("Compiling code... ");
				compiledCode = Compiler.Compile(Parser.Parse(tokenizedCode));
				console.ForegroundColor = ConsoleColor.Green;
				console.WriteLine("Done.");
			}
			catch (ParseException e)
			{
				console.ForegroundColor = ConsoleColor.Red;
				console.WriteLine("Error!");

				console.WriteLine();
				console.WriteLine($"{(options.Verbose ? sourceFile : Path.GetFileName(sourceFile))}:{e.Line}: {e.Message}");
				console.WriteLine();
				PrettyConsoleWriter.WriteCodeHighlightError(sourceCode, e, options.Verbose ? -1 : 5, console);
			}
		}

		public void WriteCompiledToDestination(IConsole console)
		{
			if (compiledCode == null)
				throw new InvalidOperationException("Code haven't been compiled yet!");
		}
	}
}