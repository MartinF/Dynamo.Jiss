using System;
using System.Collections.Generic;
using System.IO;

// IO Helper ?

namespace Dynamo.Jiss
{
	public static class FileHelper
	{
		public static void WriteFile(string filename, params string[] sources)
		{
			WriteFile(filename, sources);
		}

		public static void WriteFile(string filename, IEnumerable<string> sourceFiles, FileMode fileMode = FileMode.Create, FileAccess fileAccess = FileAccess.Write, int bufferSize = 1024)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");
			if (sourceFiles == null)
				throw new ArgumentNullException("sourceFiles");

			using (var stream = new FileStream(filename, fileMode, fileAccess))
			{
				foreach (var source in sourceFiles)
				{
					// Copy all to the stream
					using (var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read))
					{
						sourceStream.CopyTo(stream, bufferSize);
					}
				}
			}
		}
	}
}
