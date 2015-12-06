using System;
using System.Collections.Generic;

namespace openshock
{
	public static class Builtins
	{
		readonly public static Dictionary<string, Builtin> BuiltinList;

		static Builtins () {
			BuiltinList = new Dictionary<string, Builtin> ();
		}

		public static void Add<TBuiltin> () where TBuiltin: Builtin, new() {
			var builtin = new TBuiltin ();
			BuiltinList.Add (builtin.Name, builtin);
		}

		public static bool TryInvoke (string name, string[] args) {
			if (BuiltinList.ContainsKey (name)) {
				BuiltinList [name].Exec (args);
				return true;
			}
			return false;
		}
	}
}

