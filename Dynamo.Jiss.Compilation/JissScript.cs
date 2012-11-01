using System;

// Wrapper for reading the script file

namespace Dynamo.Jiss.Compilation
{
	public class JissScript : JissBase
	{
		#region Fields
		private readonly string _source;
		private readonly string _basePath;
		private readonly Language _language;
		#endregion

		#region Constructors
		public JissScript(string source, Language language, string basePath)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (basePath == null)
				throw new ArgumentNullException("basePath");

			if (!PathHelper.IsAbsolutePath(basePath))
				throw new ArgumentException("basePath should be a fully qualified path");

			_source = source;
			_language = language;
			_basePath = basePath;
		}

		// Constructor without basePath that uses som default ?

		#endregion

		#region Properties
		public override string Source { get { return _source; } }
		public override Language Language { get { return _language; } }
		public override string BasePath { get { return _basePath; } }
		#endregion
	}
}
