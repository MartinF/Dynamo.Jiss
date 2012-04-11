using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// IO Helper ?

// For small files reading full content using StringBuilder into memory is okay.
// Change if needed to be used with big files.

namespace Dynamo.Jiss
{
	public static class FileHelper
	{
		public static string GetContentCombined(params string[] files)
		{
			if (files == null)
				throw new ArgumentNullException("files");

			var builder = new StringBuilder();
			foreach (var file in files)
			{
				var content = File.ReadAllText(file);
				builder.Append(content);
			}

			return builder.ToString();
		}

		public static bool TryWriteFile(string fullname, out string content, params string[] files)
		{
			if (fullname == null)
				throw new ArgumentNullException("fullname");
			if (files == null)
				throw new ArgumentNullException("files");

			try
			{
				content = GetContentCombined(files);
				File.WriteAllText(fullname, content);
				return true;
			}
			catch (Exception)
			{
			}

			content = null;
			return false;



			//using (var stream = new FileStream(fullname, fileMode, fileAccess))
			//{
			//    foreach (var source in sourceFiles)
			//    {
			//        // Copy all to the stream
			//        using (var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read))
			//        {
			//            sourceStream.CopyTo(stream, bufferSize);
			//        }
			//    }
			//}



			// Read line by line ?

			//foreach (var file in files)
			//{
			//    using (TextWriter tw = new StreamWriter(Path.GetFullPath(output), true))
			//    {
			//        using (StreamReader tr = new StreamReader(file))
			//        {
			//            while (!tr.EndOfStream)
			//            {
			//                tw.WriteLine(tr.ReadLine());
			//            }
			//            tr.Close();
			//        }
			//        tw.Close();
			//    }
			//}
		}
	}
}
