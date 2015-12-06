using System;

namespace openshock
{
	public class AstPipeNode : AstNode
	{
		public AstPipeNode ()
		{
		}

		public override string ToString () {
			return string.Format ("Pipe: {{Empty}}");
		}
	}
}

