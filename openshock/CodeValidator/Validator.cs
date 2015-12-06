using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace openshock
{
	public class Validator
	{
		readonly List<CallContainer> CallChain;
		AstNode TopNode;

		public Validator () {
			CallChain = new List<CallContainer> ();
		}

		public void Prepare (AstNode topNode) {
			TopNode = topNode;
			CallChain.Clear ();
		}

		public void Validate () {
			for (var i = 0; i < TopNode.children.Count; i++) {
				TypeSwitch.On (TopNode.children [i])
					.Case ((AstIdentifierNode id) => CallChain.Add (new CallContainer (id.Value, id.Args)))
					.Case ((AstAndNode and) => { })
					.Case ((AstPipeNode pipe) => { CallChain.Last ().Chaining = ChainingType.Pipe; });
			}
		}

		public void Execute () {
			for (var i = 0; i < CallChain.Count; i++) {
				CallChain [i].Execute (CallChain.ElementAtOrDefault (i - 1), CallChain.ElementAtOrDefault (i + 1));
			}
		}

		public CallContainer Get (int at) {
			var item = at < 0 || CallChain.Count >= at ? null : CallChain [at];
			return item;
		}
	}
}

