using System;
using System.Linq;
using EnvDTE;

namespace Dynamo.Jiss.AddIn
{
	internal static class Extensions
	{
		public static bool WasJiss(this ProjectItem item, string oldName)
		{
			return item.IsProjectFile() && Settings.FileNameSuffix.Any(suffix => oldName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsJiss(this ProjectItem item)
		{
			return item.IsProjectFile() && Settings.FileNameSuffix.Any(suffix => item.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
		}
	}
}
