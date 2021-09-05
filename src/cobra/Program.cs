using System;
using System.IO;
using System.Collections.Generic;
using Cobra;

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
			Cobra.Scanner scanner = new Cobra.Scanner(source);
			List<Cobra.Token> tokens = scanner.ScanTokens();

			foreach (Cobra.Token token in tokens)
			{
				Console.WriteLine(token);
			}

		}

		static void Main(string[] args)
		{
			Expression expression = new Binary(
				new Unary(
					new Cobra.Token(Cobra.TokenType.MINUS, "-", null, 1),
					new Literal(123)),
				new Cobra.Token(Cobra.TokenType.STAR, "*", null, 1),
				new Grouping(
					new Literal(45.67)));
			Console.WriteLine(new AstPrinter().print(expression));
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
