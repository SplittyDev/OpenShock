using System;
using Codeaddicts.libArgument.Attributes;

namespace openshock
{
	public class Options
	{
		[Switch ("env=home")]
		public bool env_home;

		[Switch ("style=bash")]
		public bool style_bash;

		[Switch ("style=cmd")]
		public bool style_cmd;
	}
}

