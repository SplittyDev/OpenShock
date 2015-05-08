using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace openshock
{
	public static class GitIntegration
	{
		static string git;
		static string gitdir;
		const string MATCH_CURRENT_BRANCH	= @"^\* (.*)";
		const string MATCH_AHEAD			= @"^\#.*origin/.*' by (\d+) commit.*";
		const string MATCH_DELETED			= @"deleted:";
		const string MATCH_MODIFIED			= @"modified:";
		const string MATCH_RENAMED			= @"renamed:";
		const string MATCH_ADDED			= @"new file:";
		const string MATCH_UNTRACKED		= @"Untracked files:";
		const string COLOR_BRANCH			= @"\[\033[1;30m\]";
		const string COLOR_ADDED			= @"\[\033[0;32m\]";
		const string COLOR_MODIFIED			= @"\[\033[0;33m\]";
		const string COLOR_DELETED			= @"\[\033[0;31m\]";
		const string COLOR_RESET			= @"\[\033[0m\]";

		static GitIntegration () {
			git = GetGitPath ();
		}

		public static string GeneratePrompt (bool friendly = true) {
			if (!IsRepository (Windows.WorkingDirectory) || string.IsNullOrEmpty (git))
				return string.Empty;
			var accum = new StringBuilder ();
			var branch = GetCurrentBranch ();
			bool untracked, ahead;
			int added, modified, deleted, aheadcount;
			GetCurrentStatus (out untracked, out ahead, out added, out modified, out deleted, out aheadcount);
			var addspace = friendly && !string.IsNullOrEmpty (branch);
			var space = addspace ? " " : string.Empty;
			accum.AppendFormat ("{0}[", space);
			accum.AppendFormat ("{0}{1}", COLOR_BRANCH, branch);
			accum.AppendFormat ("{0} +{1}", COLOR_ADDED, added);
			accum.AppendFormat ("{0} ~{1}", COLOR_MODIFIED, modified);
			accum.AppendFormat ("{0} -{1}", COLOR_DELETED, deleted);
			accum.Append (COLOR_RESET);
			accum.Append ("]");
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

		public static void GetCurrentStatus (out bool untracked, out bool ahead,
			out int added, out int modified, out int deleted, out int aheadcount) {
			untracked = false;
			ahead = false;
			added = 0;
			modified = 0;
			deleted = 0;
			aheadcount = 0;
			var status = GetGitStatusOutput ();
			var branchbits = status.Split (' ');
			var branch = branchbits.Last ();
			var lines = status.Replace ("\r", string.Empty).Split ('\n');
			foreach (var line in lines) {
				var match_ahead = Regex.Match (line, MATCH_AHEAD);
				if (match_ahead.Success) {
					aheadcount = Convert.ToInt32 (match_ahead.Groups[1].Value);
					ahead = true;
				}
				else if (Regex.IsMatch (line, MATCH_DELETED))
					deleted++;
				else if (Regex.IsMatch (line, MATCH_MODIFIED) || Regex.IsMatch (line, MATCH_RENAMED))
					modified++;
				else if (Regex.IsMatch (line, MATCH_ADDED))
					added++;
				else if (Regex.IsMatch (line, MATCH_UNTRACKED))
					untracked = true;
			}
		}

		public static string GetCurrentBranch () {
			var branch = string.Empty;
			var lines = GetGitBranchOutput ().Replace ("\r", string.Empty).Split ('\n');
			foreach (var line in lines) {
				var match = Regex.Match (line, MATCH_CURRENT_BRANCH);
				if (match.Success)
					branch += match.Groups[1].Value;
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
					CreateNoWindow = true,
				};
				try {
					proc.Start ();
				}
				catch {
					return string.Empty;
				}
				while (!proc.StandardOutput.EndOfStream) {
					accum.AppendLine (proc.StandardOutput.ReadLine ());
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

