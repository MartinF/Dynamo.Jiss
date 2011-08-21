using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;

// Cancel Build Event ?


// See old VsProjectItemManager

// Add File to Project - if not alreday added
// Possible to Nest below existing project item

// remove / delete from ProjectItem

// rename ?

namespace Dynamo.Jiss
{
	public static class VsHelper
	{
		public static void ShowMessageBox(string message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			MessageBox.Show(message);
		}




		#region Extension methods

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







		public static ProjectItem GetRelativeItem(this Project project, string relativePath)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (relativePath == null)
				throw new ArgumentNullException("relativePath");

			return project.ProjectItems != null ? GetRelativeItem(project.ProjectItems, relativePath) : null;
		}

		public static ProjectItem GetRelativeItem(this ProjectItems items, string relativePath)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			if (relativePath == null)
				throw new ArgumentNullException("relativePath");

			ProjectItem current = null;
			foreach (string name in relativePath.Split('\\'))
			{
				try
				{
					// ProjectItems.Item() throws when it doesn't exist, so catch the exception
					// to return null instead.
					current = items.Item(name);
				}
				catch
				{
					// If any chunk couldn't be found, fail
					return null;
				}

				items = current.ProjectItems;
			}

			return current;
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



		public static bool TryRunCustomTool(this ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var vsItem = item.Object as VSProjectItem;

			if (vsItem != null)
			{
				try
				{
					vsItem.RunCustomTool();
					return true;
				}
				catch (Exception)
				{
				}
			}

			return false;
		}

		#endregion
	}
}
