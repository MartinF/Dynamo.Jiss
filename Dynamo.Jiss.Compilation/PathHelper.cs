using System.IO;

// Move to Dynamo.Jiss project ?

namespace Dynamo.Jiss.Compilation
{
	public static class PathHelper
	{
		public static bool IsAbsolutePath(string path)
		{
			// to simple - expects x:\ ?
			return path.Length >= 3 && path[1] == Path.VolumeSeparatorChar && path[2] == Path.DirectorySeparatorChar;
		}
		
		public static string ResolveRelativePath(string basePath, string relativePath)
		{
			// Does this work ? ... come on get to work and write those tests ...

			if (basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))	// c:\ etc
				return basePath + relativePath;

			return basePath + Path.DirectorySeparatorChar + relativePath;
		}
	}
}
