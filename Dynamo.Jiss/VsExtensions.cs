using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Dynamo.Jiss
{
	public static class VsExtensions
	{
		// IsProjectFile vs. IsProjectItemFile ?

		public static bool IsProjectFile(this ProjectItem item)
		{
			return item.Kind == Constants.vsProjectItemKindPhysicalFile;
		}

		public static bool IsProjectFile(this ProjectItem item, string name, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
		{
			return item.IsProjectFile() && item.Name.Equals(name, comparisonType);
		}

		public static bool IsProjectFolder(this ProjectItem item)
		{
			return item.Kind == Constants.vsProjectItemKindPhysicalFolder;
		}

		public static bool IsProjectFolder(this ProjectItem item, string name, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
		{
			return item.IsProjectFolder() && item.Name.Equals(name, comparisonType);
		}



		// GetAllNestedItems ? Children ?

		public static IEnumerable<ProjectItem> GetAllItems(this Project project)
		{
			if (project.ProjectItems != null)
				return project.ProjectItems.GetAllItems();

			return Enumerable.Empty<ProjectItem>();
		}

		public static IEnumerable<ProjectItem> GetAllItems(this ProjectItems projectItems)
		{
			// Use Recursion to run through all items

			if (projectItems == null)		// Will it still continue when this is meet when inside recursion ?
				yield break;

			foreach (ProjectItem projectItem in projectItems)
			{
				yield return projectItem;

				foreach (ProjectItem subItem in GetAllItems(projectItem.ProjectItems))
				{
					yield return subItem;
				}
			}
		}
	}
}
