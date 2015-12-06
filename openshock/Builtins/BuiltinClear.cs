using System;

namespace openshock
{
	public class BuiltinClear : Builtin
	{
		public BuiltinClear () : base ("clear") {
		}

		public override int Exec (string[] args) {
			Console.Clear ();
			return 0;
		}
	}
}

