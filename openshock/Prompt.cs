using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace openshock
{
	public static class Prompt
	{
		static string format;
		static Dictionary<string, byte> colors;
		public static string Text { get { return ParsePrompt (format); } }

		// RedHat		[\u@\h \W]\$ 
		// Fedora		[\u@\h \W]\$
		// Fedora Git	[\u@\h \W\[\033[0;32m\]\g\[\033[0m\]]\$ 
		// Ubuntu		\u@\h:\W\$ 

		static Prompt () {
			colors = new Dictionary<string, byte> {
				{ @"\033[0;30m", (byte)ConsoleColor.Black },
				{ @"\033[1;30m", (byte)ConsoleColor.DarkGray },
				{ @"\033[0;31m", (byte)ConsoleColor.DarkRed },
				{ @"\033[1;31m", (byte)ConsoleColor.Red },
				{ @"\033[0;32m", (byte)ConsoleColor.DarkGreen },
				{ @"\033[1;32m", (byte)ConsoleColor.Green },
				{ @"\033[0;33m", (byte)ConsoleColor.DarkYellow },
				{ @"\033[1;33m", (byte)ConsoleColor.Yellow },
				{ @"\033[0;34m", (byte)ConsoleColor.DarkBlue },
				{ @"\033[1;34m", (byte)ConsoleColor.Blue },
				{ @"\033[0;35m", (byte)ConsoleColor.DarkMagenta },
				{ @"\033[1;35m", (byte)ConsoleColor.Magenta },
				{ @"\033[0;36m", (byte)ConsoleColor.DarkCyan },
				{ @"\033[1;36m", (byte)ConsoleColor.Cyan },
				{ @"\033[0;37m", (byte)ConsoleColor.Gray },
				{ @"\033[1;37m", (byte)ConsoleColor.White },
				{ @"\033[0m",	 255 },
			};
		}

		public static void SetPrompt (string format) {
			Prompt.format = format;
		}

		public static void WritePrompt () {
			var prompt = Text;
			var fc = Console.ForegroundColor;
			var i = 0;
			while (i < prompt.Length && prompt[i] != '\0') {
				if (prompt[i] == '\\' && prompt[i + 1] == '[') {
					i += 2;
					var accum = new StringBuilder ();
					while (true) {
						if (prompt[i] == '\\' && prompt[i + 1] == ']')
							break;
						accum = accum.Append (prompt[i++]);
					}
					if (colors.ContainsKey (accum.ToString ()))
						Console.ForegroundColor = colors[accum.ToString ()] < 255
							? (ConsoleColor)colors[accum.ToString ()] : fc;
					i += 2;
				}
				else {
					Console.Write (prompt[i++]);
				}
			}
			Console.ForegroundColor = fc;
		}

		public static string ParsePrompt (string format) {
			var accum = new StringBuilder (format);
			accum.Replace (@"\\", @"\1");

			#region Bash
			accum.Replace (@"\a", "\a");
			accum.Replace (@"\A", DateTime.Now.ToShortTimeString ());
			// \d
			// \e
			accum.Replace (@"\h", Environment.MachineName.ToLowerInvariant ().Split ('.')[0]);
			accum.Replace (@"\H", Environment.MachineName.ToLowerInvariant ());
			// \j
			// \l
			accum.Replace (@"\n", "\n");
			// \t
			accum.Replace (@"\t", DateTime.Now.ToLongTimeString ());
			// \T
			accum.Replace (@"\r", "\r");
			accum.Replace (@"\s", "openshock");
			accum.Replace (@"\u", Environment.UserName.ToLowerInvariant ());
			accum.Replace (@"\v", Assembly.GetExecutingAssembly ().GetName ().Version.ToString (2));
			accum.Replace (@"\V", Assembly.GetExecutingAssembly ().GetName ().Version.ToString (3));
			accum.Replace (@"\w", Windows.WorkingDirectory.Unixify ());
			accum.Replace (@"\W", Windows.WorkingDirectory.Unixify ().Tail ().Unixify ());
			// \!
			// \#
			accum.Replace (@"\$", Windows.IsAdministrator () ? "#" : "$");
			// \@
			#endregion

			#region OpenShock
			accum.Replace (@"\g", GitIntegration.GeneratePrompt ());
			accum.Replace (@"\G", GitIntegration.GeneratePrompt (false));
			accum.Replace (@"\p", Windows.WorkingDirectory.Windowsify ());
			#endregion

			accum.Replace (@"\date", DateTime.Now.ToLongDateString ());
			accum.Replace (@"\sdate", DateTime.Now.ToShortDateString ());
			accum.Replace (@"\whead", Windows.WorkingDirectory.Unixify ().Head ());
			accum.Replace (@"\1", @"\");

			return accum.ToString ();
		}
	}
}

