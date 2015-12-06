using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using System.Linq;

namespace openshock
{
	public class CallContainer
	{
		public ChainingType Chaining;

		public StringBuilder Error;
		public StringBuilder Output;
		public StringBuilder Input;

		public Command Command;

		public CallContainer (string value, string args = "", ChainingType chaining = ChainingType.None) {
			var text = string.Format ("{0} {1}", value, args);
			Command = new Command (text);
			Chaining = chaining;
			Error = new StringBuilder ();
			Output = new StringBuilder ();
			Input = new StringBuilder ();
		}

		public void Execute (CallContainer prev, CallContainer next) {
			var filename = Path.GetFileName (Command.Name);
			if (Builtins.TryInvoke (filename, Command.Args))
				return;
			var dir = Path.GetDirectoryName (Command.Name);
			var path = GetPath (filename);
			if (path != string.Empty)
				Command.Name = path;
			var procsi = new ProcessStartInfo {
				Arguments = Command.ToArgsString (),
				FileName = Command.Name,
				RedirectStandardOutput = true,
				RedirectStandardInput = true,
				UseShellExecute = false,
			};
			var proc = new Process ();
			proc.StartInfo = procsi;
			proc.OutputDataReceived += (sender, e) => {
				string displayData;
				try {
					var lines = e.Data.Split (' ');
					for (var i = 0; i < lines.Length; i++)
						lines [i] = lines [i].Trim (' ', '\r', '\n');
					displayData = string.Join (string.Empty, lines);
				} catch { return; }
				switch (Chaining) {
				case ChainingType.None:
					Console.Out.WriteLine (displayData);
					Output.AppendLine (displayData);
					break;
				case ChainingType.Pipe:
					if (next != null)
						next.Input.AppendLine (displayData);
					Output.AppendLine (displayData);
					break;
				}
			};
			proc.EnableRaisingEvents = true;
			try {
				proc.Start ();
				if (Input.ToString () != string.Empty) {
					proc.StandardInput.WriteLine (Input);
					proc.StandardInput.Flush ();
					proc.StandardInput.Close ();
				}
				proc.BeginOutputReadLine ();
				if (Input.ToString () == string.Empty) {
					bool inputClosed = false;
					while (!inputClosed && !proc.HasExited) {
						if (Console.KeyAvailable) {
							var key = Console.ReadKey (true).KeyChar;
							switch ((int) key) {
							case 0x3:
							case 0x4:
								inputClosed = true;
								proc.StandardInput.Close ();
								break;
							case '\n':
								Console.Out.WriteLine ();
								break;
							default:
								Console.Out.Write (key);
								proc.StandardInput.Write (key);
								break;
							}
							if (!inputClosed)
								proc.StandardInput.Flush ();
						}
					}
				}
				proc.WaitForExit();
				if (Chaining == ChainingType.None && Output.ToString ().Skip (Output.Length - 3).Take (2).All (c => c != 10))
					Console.Out.WriteLine ();
			} catch (Exception ex) {
				Console.Error.WriteLine (ex.Message);
			}
		}

		public static string GetPath (string filename) {
			string procname = filename;
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
	}

	public enum ChainingType {
		None = 0,
		Pipe,
	}
}

