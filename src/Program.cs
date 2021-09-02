using System;
using System.IO;
using System.Collections.Generic;

namespace cobra
{
	class Program
	{
		static void runFile(String path)
		{
			String text = File.ReadAllText(Path.GetFullPath(path));
			Console.WriteLine("working on following text:\n" + text);
			Console.WriteLine("----------------");

			run(text);
		}

		private static void runPrompt()
		{
			for (; ; )
			{
				Console.Write('>');
				run(Console.ReadLine());
			}
		}

		private static void run(String source)
		{
			var tokens = source.Split(new char[] { ' ', ',', '?', '\n' }, StringSplitOptions.TrimEntries);
			Console.WriteLine("tokenised text:");
			foreach (var token in tokens)
			{
				Console.WriteLine(token);
			}
			Console.WriteLine("----------------");

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
