using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openshock
{
	public class Parser
	{
		List<Token> tokens;
		List<AstNode> nodes;
		int pos;

		public Parser () {
			nodes = new List<AstNode> ();
		}

		public void Prepare (List<Token> tokenlist) {
			if (nodes != null)
				nodes.Clear ();
			tokens = tokenlist;
			pos = -1;
		}

		public AstNode Parse () {
			var code = new AstCodeblockNode ();
			bool abort = false;
			while (!abort && CanAdvance ()) {
				TypeSwitch.On (Peek ())
					.Case ((TK_IDENT id) => code.children.Add (ParseIdentifier ()))
					.Case ((TK_PIPE pipe) => code.children.Add (ParsePipe ()))
					.Case ((TK_AND and) => Read ())
					.Default (tk => {
						Console.Error.WriteLine ("Error: Unexpeced token type: {0}", tk.GetType ().Name);
						abort = true;
				});
			}
			return code;
		}

		AstNode ParseIdentifier () {
			var value = Accept<TK_IDENT> ().value;
			var accum = new StringBuilder ();
			while (Check<TK_IDENT> ())
				accum.AppendFormat ("{0} ", Accept<TK_IDENT> ().value);
			var node = new AstIdentifierNode (value, accum.ToString ().Trim ());
			return node;
		}

		AstNode ParsePipe () {
			Accept<TK_PIPE> ();
			return new AstPipeNode ();
		}

		bool CanAdvance (int n = 1) {
			return tokens.Count > pos + n;
		}

		Token Peek (int n = 1) {
			return CanAdvance () ? tokens [pos + n] : null;
		}

		Token Read () {
			return CanAdvance () ? tokens [++pos] : null;
		}

		bool Check<TMatch> () where TMatch : Token {
			return Peek () is TMatch;
		}

		TMatch Accept<TMatch> () where TMatch : Token {
			return (TMatch)Read ();
		}
	}
}

