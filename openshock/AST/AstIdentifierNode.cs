using System;

namespace openshock
{
	public class AstIdentifierNode : AstNode
	{
		public string Value = string.Empty;
		public string Args = string.Empty;

		public AstIdentifierNode (string value, string args) {
			Value = value;
			Args = args;
		}

		public override string ToString () {
			return string.Format ("Identifier: {{Value: '{0}'; Args: '{1}'}}",
				Value != string.Empty ? Value : "Empty",
				Args != string.Empty ? Args : "Empty");
		}
	}
}

