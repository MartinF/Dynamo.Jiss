using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

// Make implementations so there is a default and easy way to handle exceptions instead of having to write it in every script ?

namespace Dynamo.Jiss
{
	public static class ExecuteHelper
	{
		// Fields
		private static string _javaInstallPath;


		public static bool TryExecuteJava(string command, out ProcessResult result)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			var javaPath = GetJavaInstallationPath() + "\\bin\\";
			var filename = javaPath + "java.exe";

			return TryExecute(filename, command, out result);
		}

		public static bool TryExecute(string filename, string args, out ProcessResult result)
		{
			// Set HomeDirectory for shorter paths ?

			if (filename == null)
				throw new ArgumentNullException("filename");

			if (args == null)
				args = string.Empty;

			string output = string.Empty;
			string error = string.Empty;
			int exitCode = -1;

			var processStartInfo = new ProcessStartInfo(filename, args)
			{
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
				// WorkingDirectory ?
			};

			// Try Execute
			try
			{
				using (var process = Process.Start(processStartInfo))
				{
					output = process.StandardOutput.ReadToEnd();
					error = process.StandardError.ReadToEnd();
					exitCode = process.ExitCode;

					//if (!process.HasExited)	// Needed ? Dispose will probably fix this ?
					//    process.Kill();
				}
			}
			catch (Exception e)
			{
				// Store exeception in ExecuteResult ?
				// Log error somehow ? try/catch in caller instead ?
				// Output to output, or ErrorList ?
			}

			result = new ProcessResult(output, error, exitCode);
			return result.ExitCode == 0;
		}



		public static string GetJavaInstallationPath()
		{
			if (_javaInstallPath == null)
			{
				// What is returned if it cant be found ?

				var path = GetJavaInstallationPathFromRegistry(RegistryView.Registry32);

				if (string.IsNullOrEmpty(path))
					path = GetJavaInstallationPathFromRegistry(RegistryView.Registry64);

				if (string.IsNullOrEmpty(path))
					throw new Exception("Java installation cannot be found"); // ???

				_javaInstallPath = path;
			}

			return _javaInstallPath;
		}

		private static string GetJavaInstallationPathFromRegistry(RegistryView registryView)
		{
			// http://stackoverflow.com/questions/3038140/how-to-determine-windows-java-installation-location

			string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(javaKey))
			{
				string currentVersion = baseKey.GetValue("CurrentVersion").ToString();
				using (var homeKey = baseKey.OpenSubKey(currentVersion))
				{
					return homeKey.GetValue("JavaHome").ToString();	// +"\\bin\\"; ???
				}
			}
		}






		public static void ExecuteWithDelay(int ms, EventHandler action)
		{
			var tmp = new Timer { Interval = ms };
			tmp.Tick += new EventHandler((o, e) => tmp.Enabled = false);
			tmp.Tick += action;
			tmp.Enabled = true;
		}
	}



	public class ProcessResult
	{
		public ProcessResult(string output, string error, int exitcode)
		{
			Output = output;
			Error = error;
			ExitCode = exitcode;
		}

		public string Output { get; private set; }
		public string Error { get; private set; }
		public int ExitCode { get; private set; }
	}
}
