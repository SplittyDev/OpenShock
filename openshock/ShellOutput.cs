using System;
using System.Collections.Generic;

namespace openshock
{
	public static class ShellOutput
	{
		static Dictionary<string, byte> colors = new Dictionary<string, byte> {
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

		public static void WriteLine (string line) {
		}
	}
}

