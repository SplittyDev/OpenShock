using System;

namespace openshock
{
	public class TK_IDENT : Token
	{
		public string value;

		public override void SetArgs (object args) {
			value = (string)args;
		}

		public override string ToString () {
			return string.Format ("TK_IDENT: {{Value: {0}}}", value);
		}
	}
}

