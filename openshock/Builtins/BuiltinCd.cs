using System;
using System.IO;

namespace openshock
{
	public class BuiltinCd : Builtin
	{
		public BuiltinCd () : base ("cd") {
		}

		public override int Exec (string[] args) {
			if (args.Length == 0) {
				Console.WriteLine (Windows.WorkingDirectory);
				return 0;
			}
			string path;
			try {
				path = Path.GetFullPath (args [0]);
				Windows.WorkingDirectory = path;
			} catch (Exception) {
				Console.Error.WriteLine ("Error: Invalid path: '{0}'", args);
				return 1;
			}
			return 0;
		}
	}
}

