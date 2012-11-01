using System;
using System.IO;

// Wrapper for reading the script file


// Let this inherit from JissScript !?

namespace Dynamo.Jiss.Compilation
{
	public class JissFile : JissBase
	{
		#region Fields
		private readonly string _source;
		private readonly Language _language;
		private readonly string _basePath;
		private readonly string _filename;
		private readonly string _fullname;
		#endregion

		#region Constructor

		// why not have constructor that take fileName and then figure source, language and soo on out by itself
		// Could just parse the data / get source first time needed / requested

		public JissFile(string source, Language language, string fileFullName)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (language == Language.None)
				throw new NotSupportedException("Language None is not supported");
			if (fileFullName == null)
				throw new ArgumentNullException("fileFullName");
			
			// Check if valid fileFullName? - or only take in fileFullName and do the loading in the constructor instead of using the static helper?

			_source = source;
			_language = language;
			_basePath = Path.GetDirectoryName(fileFullName);
			_filename = Path.GetFileName(fileFullName);
			_fullname = fileFullName;
		}
		#endregion

		#region Properties
		public override string Source { get { return _source; } }
		public override Language Language { get { return _language; } }
		public override string BasePath { get { return _basePath; } }
		public string Filename { get { return _filename; } }			// Name ?
		public string FullName { get { return _fullname; } }
		#endregion

		#region Static constructor helpers
		public static JissFile Create(string filename)
		{
			// Read / Create ? 

			if (filename == null)
				throw new ArgumentNullException("filename");

			var language = LanguageHelper.GetLanguage(filename);

			// Dont check this here - let it happen in the constructor ?
			// Or let the GetLanguage fail ?

			//if (language == Language.None)
			//    throw new NotSupportedException("File extension " + _fileInfo.Extension + " is not supported");


			// check file.Exists ? should just throw exception any way ? so who cares

			var source = File.ReadAllText(filename);

			// is there any source ? - if not throw ? 

			return new JissFile(source, language, filename);
		}
		#endregion
	}
}
