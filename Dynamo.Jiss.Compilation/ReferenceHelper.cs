using System.Collections.Generic;

// Should References depend on the framework version ?

namespace Dynamo.Jiss.Compilation
{
	public static class ReferenceHelper
	{
		public static IEnumerable<string> GetFrameworkReferences()		// GetDefaultFrameworkReferences - GetGacReferences - GetDefaultGacs !?
		{
			return new string[]
			{
				"System.dll",
				"System.Core.dll",
				"System.Web.dll",
				"Microsoft.CSharp.dll",
				"System.Windows.Forms.dll",
				"System.Xml.dll",
				"System.Xml.Linq.dll",
				"System.Data.dll"
			};
		}

		public static string GetJissLibraryReference()
		{
			return typeof(IEventScript).Assembly.Location;
		}
	}
}
