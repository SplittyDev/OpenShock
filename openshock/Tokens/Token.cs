using System;

namespace openshock
{
	public abstract class Token
	{
		public virtual void SetArgs (object args) {
		}

		public static TToken Create<TToken> () where TToken : Token, new () {
			return new TToken ();
		}

		public static TToken Create<TToken> (object args) where TToken : Token, new () {
			var token = new TToken ();
			token.SetArgs (args);
			return token;
		}
	}
}

