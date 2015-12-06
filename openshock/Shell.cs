using System;
using System.Reflection;

namespace openshock
{
	public static class Shell
	{
		static bool exit;
		static Scanner scanner;
		static Parser parser;
		static Validator validator;

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
			//Prompt.SetPrompt (@"\u@\h \W\g\$ ");
			var version = Assembly.GetEntryAssembly ().GetName ().Version;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine ("Welcome to shocksh (version {0})\n\n", version.ToString (3));
			Console.WriteLine ("Run 'shock help' to display the help.");
			Console.WriteLine ("Run 'shock help shock' to display startup options.\n");
			Prompt.SetPrompt (@"[\[\033[0;32m\]\u@\h\[\033[0m\] \[\033[0;33m\]\W \[\033[0m\]\G]\$ ");
			scanner = new Scanner ();
			parser = new Parser ();
			validator = new Validator ();
			Builtins.Add<BuiltinCd> ();
			Builtins.Add<BuiltinClear> ();
			Builtins.Add<BuiltinExit> ();
		}

		static void ParseCommand (string command) {
			var tokens = scanner.Scan (command);
			parser.Prepare (tokens);
			var topNode = parser.Parse ();
			// DumpAst (topNode);
			validator.Prepare (topNode);
			validator.Validate ();
			validator.Execute ();
		}

		static void DumpAst (AstNode node, int depth = 0) {
			Console.WriteLine ("{0}* {1}", "".PadLeft (depth * 2, ' '), node);
			foreach (var child in node.children)
				DumpAst (child, depth + 1);
		}
	}
}

