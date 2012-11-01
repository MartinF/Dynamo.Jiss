using System.Globalization;
using System.IO;

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

			var path = basePath;

			// if not basePath ends with separator \ - c: etc - add it
			if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))	
				path += Path.DirectorySeparatorChar;

			return path + relativePath;
		}
	}
}
