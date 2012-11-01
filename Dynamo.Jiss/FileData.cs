using System;

namespace Dynamo.Jiss
{
	public class FileData
	{
		public FileData(string filename, string content)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");
			if (content == null)
				throw new ArgumentNullException("content");

			Filename = filename;
			Content = content;
		}

		public string Filename { get; private set; }
		public string Content { get; set; }
	}
}
