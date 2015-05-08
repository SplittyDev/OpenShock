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
			pos = 0;
		}

		public AstNode Parse () {
			return null;
		}

		public AstNode ParseIdentifier () {
			var command = new Command ();
			var args = new StringBuilder ();
			while (MatchToken<TK_IDENT> (PeekToken ())) {
				args.Append ((PeekToken () as TK_IDENT).value);
				NextToken ();
			}
			return null;
		}

		public bool MatchToken<TMatch> (Token src) {
			return src is TMatch;
		}

		public Token PeekToken (int lookahead = 1) {
			return tokens[pos + lookahead];
		}

		public Token NextToken () {
			return tokens[++pos];
		}
	}
}

