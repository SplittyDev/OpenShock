using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openshock
{
	public class Scanner
	{
		string src;
		int pos;
		List<Token> tokens;

		public Scanner () {
			tokens = new List<Token> ();
		}

		public List<Token> Scan (string command) {
			if (tokens != null)
				tokens.Clear ();
			src = command;
			pos = -1;
			while (Peek () != -1) {
				while (Peek () != -1 && char.IsWhiteSpace ((char)Peek ()))
					pos++;
				switch ((char)Peek ()) {
					case '>':
						Next ();
						ScanRedirection ();
						break;
					case '|':
						Next ();
						ScanPipe ();
						break;
					default:
						ScanIdent ();
						break;
				}
			}
			return tokens;
		}

		public void ScanRedirection () {
			if (Peek () == '>') {
				Next ();
				tokens.Add (Token.Create<TK_RDIR_A> ());
			}
			else
				tokens.Add (Token.Create<TK_RDIR> ());
		}

		public void ScanPipe () {
			tokens.Add (Token.Create<TK_PIPE> ());
		}

		public void ScanIdent () {
			var accum = new StringBuilder ();
			int c = Next ();
			while (c != -1 && !char.IsWhiteSpace ((char)c)) {
				accum.Append ((char)c);
				c = Next ();
			}
			tokens.Add (Token.Create<TK_IDENT> (accum.ToString ()));
		}

		public int Next () {
			return Peek () != -1 ? src[++pos] : -1;
		}

		public int Peek (int lookahead = 1) {
			return src.Length > (pos + lookahead) ? src[pos + lookahead] : -1;
		}
	}
}

