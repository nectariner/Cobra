using System;
using System.IO;
using System.Collections.Generic;
using static Cobra.src.Scanner;
using static Cobra.src.Token;

namespace cobra
{
	class Program
	{
		static bool hadError = false;
		static public void error(int line, string message)
		{
			report(line, "", message);
		}

		static void report(int line, string where, string message)
		{
			Console.WriteLine("[Line " + line + "] Error " + where + ": " + message);
			hadError = true;
		}

		static void runFile(String path)
		{
			String text = File.ReadAllText(Path.GetFullPath(path));
			Console.WriteLine("working on following text:\n" + text);
			Console.WriteLine("----------------");

			run(text);
		}

		private static void runPrompt()
		{
			while (true)
			{
				Console.Write('>');
				run(Console.ReadLine());
				hadError = false;
			}
		}

		private static void run(String source)
		{
			Cobra.src.Scanner scanner = new Cobra.src.Scanner(source);
			List<Cobra.src.Token> tokens = scanner.ScanTokens();

			foreach (Cobra.src.Token token in tokens)
			{
				Console.WriteLine(token);
			}

		}

		static void Main(string[] args)
		{
			Console.WriteLine(args.Length);
			if (args.Length > 1)
			{
				Console.WriteLine("Usage: cobra [script]");
				//return;
			}
			else if (args.Length == 1)
			{
				runFile(args[0]);
			}
			else
			{
				runPrompt();
			}
		}
	}
}
