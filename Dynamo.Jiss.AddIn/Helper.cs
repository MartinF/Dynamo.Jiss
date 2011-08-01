using System.Collections.Generic;
using System.Linq;
using Dynamo.Jiss.Compilation;
using EnvDTE;
using EnvDTE80;

namespace Dynamo.Jiss.AddIn
{
	internal static class Helper
	{
		public static IEnumerable<string> GetDefaultReferences()
		{
			// Framework references
			foreach (var frameworkReference in ReferenceHelper.GetFrameworkReferences())
			{
				yield return frameworkReference;
			}

			// Visual studio references - include 90 + 10 + the one for running CustomTool ?!?!?!?!? 
			yield return typeof(Project).Assembly.Location;
			yield return typeof(Events2).Assembly.Location;

			// Dynamo Jiss Lib
			yield return ReferenceHelper.GetJissLibraryReference();
		}
	}
}
