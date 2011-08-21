using System.IO;
using EnvDTE;

namespace Dynamo.Jiss
{
	public static class PathHelper
	{
		// Extension methods

		public static string GetRelativePath(this Project project, string path)
		{
			var projectPath = Path.GetDirectoryName(project.FullName);
			return path.Replace(projectPath, "");
		}

		public static string GetRelativePath(this Project project, ProjectItem item)
		{
			var itemPath = item.FileNames[0];
			return GetRelativePath(project, itemPath);
		}

		public static string GetRenamedPath(this ProjectItem projectItem, string oldName)
		{
			return Path.GetDirectoryName(projectItem.FileNames[0]) + "\\" + oldName;
		}
	}
}
