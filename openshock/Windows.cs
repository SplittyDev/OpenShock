using System;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace openshock
{
	public static class Windows
	{
		public const char SEPARATOR_WINDOWS = '\\';
		public const char SEPARATOR_UNIX = '/';

		public static bool IsAdministrator ()
		{
			var user = WindowsIdentity.GetCurrent ();
			var principal = new WindowsPrincipal (user);
			return principal.IsInRole (WindowsBuiltInRole.Administrator);
		}

		public static string WorkingDirectory {
			get { return Environment.CurrentDirectory; }
			set { Environment.CurrentDirectory = value; }
		}

		public static string HomeDirectory {
			get {
				return Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
			}
		}

		public static string Head (this string path) {
			var wd = WorkingDirectory;
			if (wd.Contains (SEPARATOR_WINDOWS)) {
				var wds = wd.Split (SEPARATOR_WINDOWS);
				return string.Join (SEPARATOR_WINDOWS.ToString (), wds.Take (wds.Length - 1));
			}
			else if (wd.Contains (SEPARATOR_UNIX)) {
				var wds = wd.Split (SEPARATOR_UNIX);
				return string.Join (SEPARATOR_UNIX.ToString (), wds.Take (wds.Length - 1));
			}
			return string.Empty;
		}

		public static string Tail (this string path) {
			var wd = WorkingDirectory;
			if (wd.Contains (SEPARATOR_WINDOWS))
				return wd.Split (SEPARATOR_WINDOWS).Last ();
			else if (wd.Contains (SEPARATOR_UNIX))
				return wd.Split (SEPARATOR_UNIX).Last ();
			return wd;
		}

		public static string[] GetSearchPathArray () {
			var winpathm = Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.Machine);
			var winpathu = Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.User);
			var shockpath = Environment.GetEnvironmentVariable ("SHOCKPATH", EnvironmentVariableTarget.User) ?? string.Empty;
			var winpathsm = winpathm.Split (';');
			var winpathsu = winpathu.Split (';');
			var shockpaths = shockpath.Split (';');
			var arr = new string[winpathsm.Length + winpathsu.Length + shockpaths.Length];
			Array.Copy (winpathsm, arr, winpathsm.Length);
			Array.Copy (winpathsu, 0, arr, winpathsm.Length, winpathsu.Length);
			Array.Copy (shockpaths, 0, arr, winpathsm.Length + winpathsu.Length, shockpaths.Length);
			return arr.Distinct ().ToArray ();
		}

		public static string GetSearchPathString () {
			return string.Join (";", GetSearchPathArray ());
		}

		public static string PureWindowsify (this string path) {
			return path.ToLowerInvariant ()
				.Replace (SEPARATOR_UNIX, SEPARATOR_WINDOWS);
		}

		public static string Windowsify (this string path) {
			return path.PureWindowsify ().Replace ("~", HomeDirectory.PureWindowsify ());
		}

		public static string PureUnifixy (this string path) {
			return path.ToLowerInvariant ()
				.Replace (SEPARATOR_WINDOWS, SEPARATOR_UNIX);
		}

		public static string Unixify (this string path) {
			return path.PureUnifixy ().Replace (HomeDirectory.PureUnifixy (), "~");
		}
	}
}

