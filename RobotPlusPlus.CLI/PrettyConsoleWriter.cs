﻿using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using RobotPlusPlus.Exceptions;
using RobotPlusPlus.Tokenizing.Tokens;
using RobotPlusPlus.Tokenizing.Tokens.Literals;

namespace RobotPlusPlus.CLI
{
	public class PrettyConsoleWriter
	{
		public static void WriteCodeHighlightError(string sourceCode, ParseException e, int radius, IConsole console)
		{
			console.ResetColor();

			string[] lines = sourceCode.Split('\n');
			if (radius < 0) radius = lines.Length;

			int start = Math.Max(0, e.Line - radius);
			int stop = Math.Min(lines.Length, e.Line + radius + 1);
			int totalLinesWidth = (int)Math.Ceiling(Math.Log10(lines.Length)) + 1;

			for (int i = start; i < stop; i++)
			{
				if (i + 1 == e.Line)
					console.BackgroundColor = ConsoleColor.DarkRed;
				else
					console.ResetColor();

				console.ForegroundColor = ConsoleColor.DarkGray;
				console.Write((i+1).ToString().PadLeft(totalLinesWidth) + ':');

				console.ForegroundColor = ConsoleColor.Gray;
				console.WriteLine(lines[i]);
			}

			console.ResetColor();
		}

		public static (string code, ConsoleColor color)[] ColorizeTokens(Token[] tokenizedCode)
		{
			return tokenizedCode.Select(token =>
			{
				string output;
				ConsoleColor color;

				switch (token)
				{
					case LiteralNumber num:
						color = ConsoleColor.Yellow;
						output = token.SourceCode;
						break;

					case LiteralString str:
						color = ConsoleColor.Green;
						output = token.SourceCode;
						break;

					case Statement st:
					case LiteralKeyword kw:
					case Operator op:
						color = ConsoleColor.Magenta;
						output = token.SourceCode;
						break;

					case Comment com:
						color = ConsoleColor.DarkGreen;
						output = token.SourceCode;
						break;

					default:
						color = ConsoleColor.White;
						output = token.SourceCode;
						break;
				}

				return (output, color);
			}).ToArray();
		}

		public static void WriteCodeToConsole(Token[] tokenizedCode, IConsole console)
		{
			(string code, ConsoleColor color)[] colorizedTokens = ColorizeTokens(tokenizedCode);
			int totalLines = tokenizedCode.Sum(token => token.NewLines);
			int totalLinesWidth = (int)Math.Ceiling(Math.Log10(totalLines)) + 1;
			var lineNumb = 0;

			foreach ((string code, ConsoleColor color) in colorizedTokens)
			{
				string[] lines = code.Split('\n');
				for (var i = 0; i < lines.Length; i++)
				{
					if (i > 0 || lineNumb == 0)
					{
						if (lineNumb > 0) Console.WriteLine();
						lineNumb++;
						
						console.ForegroundColor = ConsoleColor.DarkGray;
						console.Write(lineNumb.ToString().PadLeft(totalLinesWidth) + ": ");
					}

					console.ForegroundColor = color;
					console.Write(lines[i]);
				}
			}

			console.WriteLine();
			console.ResetColor();
		}

	}
}