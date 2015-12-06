using System;

namespace openshock
{
	public class BuiltinExit : Builtin
	{
		public BuiltinExit () : base ("exit") {
		}

		public override int Exec (string[] args) {
			int exitCode = 0;
			if (args.Length > 0)
				int.TryParse (args [0], out exitCode);
			Environment.Exit (exitCode);
			return 0;
		}
	}
}

