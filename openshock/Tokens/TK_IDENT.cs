using System;

namespace openshock
{
	public class TK_IDENT : Token
	{
		public string value;

		public override void SetArgs (object args) {
			value = (string)args;
		}
	}
}

