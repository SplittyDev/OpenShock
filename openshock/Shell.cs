using System;

namespace openshock
{
	public static class Shell
	{
		static bool exit;
		static Scanner scanner;
		static Parser parser;

		public delegate void SignalExit ();
		public static event SignalExit OnExit;

		public static void Run () {
			Prepare ();
			while (!exit) {
				Prompt.WritePrompt ();
				var cmd = Console.ReadLine ();
				ParseCommand (cmd);
			}
		}

		static void Exit () {
			OnExit ();
		}

		static void Prepare () {
			exit = false;
			Prompt.SetPrompt (@"\u@\h \W\g\$ ");
			scanner = new Scanner ();
			parser = new Parser ();
		}

		static void ParseCommand (string command) {
			var tokens = scanner.Scan (command);
			foreach (var token in tokens) {
				Console.WriteLine (token);
			}
			parser.Prepare (tokens);
			//var astnode = parser.Parse ();
		}
	}
}

