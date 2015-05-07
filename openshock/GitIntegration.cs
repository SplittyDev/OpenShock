using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace openshock
{
	public static class GitIntegration
	{
		static string git;
		static string gitdir;
		const string MATCH_CURRENT_BRANCH = @"^\* (.*)";

		static GitIntegration () {
			git = GetGitPath ();
		}

		public static string GeneratePrompt (bool friendly = true) {
			if (!IsRepository (Windows.WorkingDirectory) || string.IsNullOrEmpty (git))
				return string.Empty;
			var accum = new StringBuilder ();
			var branch = GetCurrentBranch ();
			accum.AppendFormat ("{0}{1}", friendly && !string.IsNullOrEmpty (branch)? " " : string.Empty, branch);
			return accum.ToString ();
		}

		public static string GetGitPath () {
			const string procname = "git";
			var search = Windows.GetSearchPathArray ();
			return new Func<string> (() => {
				var _search = new List<string> ();
				_search.Add (Environment.CurrentDirectory);
				_search.Add (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), "coreutils", "bin"));
				_search.AddRange (search);
				var _path = string.Empty;
				foreach (var cur in _search)
					if (File.Exists (Path.Combine (cur, string.Format ("{0}.exe", procname))))
						return Path.Combine (cur, string.Format ("{0}.exe", procname));
					else if (File.Exists (Path.Combine (cur, procname)))
						return Path.Combine (cur, procname);
				return string.Empty;
			}) ();
		}

		public static string GetCurrentBranch () {
			var branch = string.Empty;
			var lines = GetGitBranchOutput ().Split ('\n');
			foreach (var line in lines) {
				Console.WriteLine ("Branch: {0}", line);
				if (Regex.IsMatch (line, MATCH_CURRENT_BRANCH))
					Console.WriteLine ("Success!");
			}
			return branch;
		}

		public static string GetGitBranchOutput () {
			return GetGitOutput ("branch");
		}

		public static string GetGitStatusOutput () {
			return GetGitOutput ("status");
		}

		public static string GetGitOutput (string args) {
			var accum = new StringBuilder ();
			using (var proc = new Process ()) {
				proc.StartInfo = new ProcessStartInfo {
					FileName = git,
					Arguments = args,
					WorkingDirectory = gitdir,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};
				try {
					proc.Start ();
				}
				catch {
					return string.Empty;
				}
				while (!proc.StandardOutput.EndOfStream) {
					accum.Append (proc.StandardOutput.ReadLine ());
				}
				proc.WaitForExit (100);
			}
			return accum.ToString ();
		}

		public static bool IsInRepository (string path) {
			bool found = false;
			bool error = false;
			var prevpath = path;
			while (!found && !error) {
				try {
					var dir = Directory.GetParent (prevpath);
					prevpath = dir.FullName;
					if (!dir.Exists)
						continue;
					if (Directory.Exists (Path.Combine (dir.FullName, ".git"))) {
						gitdir = dir.FullName;
						found = true;
					}
				}
				catch {
					error = true;
				}
			}
			return found;
		}

		public static bool IsRepository (string path) {
			var exists = Directory.Exists (Path.Combine (path, ".git"));
			if (exists) {
				gitdir = Path.GetFullPath (path);
				return true;
			}
			else
				return IsInRepository (Path.GetFullPath (path));
		}
	}
}

