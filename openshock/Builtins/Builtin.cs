using System;

namespace openshock
{
	public abstract class Builtin
	{
		public string Name;

		protected Builtin (string name) {
			Name = name;
		}

		public abstract int Exec (string[] args);
	}
}

