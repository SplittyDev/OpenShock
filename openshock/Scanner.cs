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

		public Scanner ()
		{
			tokens = new List<Token> ();
		}

		public List<Token> Scan (string command)
		{
			if (tokens != null)
				tokens.Clear ();
			src = command;
			pos = -1;
			while (Peek () != -1) {
				while (Peek () != -1 && char.IsWhiteSpace ((char)Peek ()))
					pos++;
				switch (Peek ()) {
				case '<':
					Next ();
					ScanRedirectionIn ();
					break;
				case '>':
					Next ();
					ScanRedirectionOut ();
					break;
				case '|':
					Next ();
					ScanPipe ();
					break;
				case '&':
					Next ();
					ScanAnd ();
					break;
				default:
					ScanIdent ();
					break;
				}
			}
			return tokens;
		}

		public void ScanRedirectionIn ()
		{
			if (Peek () == '>') {
				Next ();
				tokens.Add (Token.Create<TK_RDIR_IN_A> ());
			} else
				tokens.Add (Token.Create<TK_RDIR_IN> ());
		}

		public void ScanRedirectionOut ()
		{
			if (Peek () == '>') {
				Next ();
				tokens.Add (Token.Create<TK_RDIR_OUT_A> ());
			} else
				tokens.Add (Token.Create<TK_RDIR_OUT> ());
		}

		public void ScanPipe ()
		{
			if (Peek () == '|') {
				Next ();
				tokens.Add (Token.Create<TK_OR> ());
			} else
				tokens.Add (Token.Create<TK_PIPE> ());
		}

		public void ScanAnd () {
			if (Peek () == '&') {
				Next ();
				tokens.Add (Token.Create<TK_AND> ());
			} else
				Console.Error.WriteLine ("Error: Unexpected symbol: '&'");
		}

		public void ScanIdent ()
		{
			var accum = new StringBuilder ();
			int c = Next ();
			while (c != -1 && !char.IsWhiteSpace ((char)c)) {
				accum.Append ((char)c);
				c = Next ();
			}
			tokens.Add (Token.Create<TK_IDENT> (accum.ToString ()));
		}

		public int Next ()
		{
			return Peek () != -1 ? src [++pos] : -1;
		}

		public int Peek (int lookahead = 1)
		{
			return src.Length > (pos + lookahead) ? src [pos + lookahead] : -1;
		}
	}
}

