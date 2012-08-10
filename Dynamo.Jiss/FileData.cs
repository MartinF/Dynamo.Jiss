using System;

namespace Dynamo.Jiss
{
	public class FileData
	{
		public FileData(string name, string content)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");

			Name = name;
			Content = content;
		}

		public string Name { get; private set; }
		public string Content { get; set; }
	}
}
