using System;

namespace openshock
{
	public static class Shell
	{
		static bool exit;

		public delegate void SignalExit ();
		public static event SignalExit OnExit;

		public static void Run () {
			Prepare ();
			while (!exit) {
				Prompt.WritePrompt ();
				ParseCommand ();
			}
		}

		static void Exit () {
			OnExit ();
		}

		static void Prepare () {
			exit = false;
			Prompt.SetPrompt (@"[\u@\h \W\[\033[0;32m\]\g\[\033[0m\]]\$ ");
		}

		static void ParseCommand () {
			var prompt = Console.ReadLine ();
			if (prompt == "exit")
				Exit ();
		}
	}
}

