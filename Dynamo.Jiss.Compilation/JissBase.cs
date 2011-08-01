using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Implements core logic for reading the header of the source

// JissScriptBase ?

// LoadHeader and analyse immediatly - but only possible if not abstract - but merged into 1 single file etc.

namespace Dynamo.Jiss.Compilation
{
	public abstract class JissBase
	{
		#region Fields
		private bool _isAnalyzed = false;
		private List<string> _gacs;
		private List<string> _references;
		private List<string> _includes;
		#endregion

		#region Properties - abstract
		public abstract string Source { get; }
		public abstract Language Language { get; }
		public abstract string BasePath { get; }		// BaseDirectory ? Root ? Home ? what ?
		#endregion

		#region Methods
		//public string GetEntryPoint()
		//{		
		//}

		public IEnumerable<string> GetGacs()
		{
			AnalyzeHeader();
			return _gacs;
		}

		public IEnumerable<string> GetReferences()
		{
			AnalyzeHeader();
			return _references;
		}

		public IEnumerable<string> GetIncludes()
		{
			AnalyzeHeader();
			return _includes;
		}

		private string GetHeader()
		{
			// From start until first line that doesnt start with //
			// What if the file starts with "#using Section..." - should be supported - but not now
			// make possible to only open file and read the start of the file to get the header instead of reading full source - if filenames are used instead - seperate in two implementations ?

			StringBuilder builder = new StringBuilder();
			string line;

			using (var reader = new StringReader(Source))
			{
				// Loop through every line
				while ((line = reader.ReadLine()) != null)
				{
					line = line.Trim();

					if (!line.StartsWith("//"))
						break;

					builder.Append(line);
				}
			}

			return builder.ToString();
		}

		private void AnalyzeHeader()
		{
			// Write better regular expression some time ...

			// Support:
			// gac system.dll;
			// reference whateverererer.dll;
			// include somefile\alalalala.cs;
			// entry Main;

			// Analyze if it have not already happend
			if (!_isAnalyzed)
			{
				_isAnalyzed = true;

				_gacs = new List<string>();
				_references = new List<string>();
				_includes = new List<string>();

				var header = GetHeader();

				if (header != string.Empty) // check length ?
				{
					// Get Gacs
					var gacs = CaptureMatches(header, @"gac (?<gac>[\w|\.|\-]+);", "gac");
					_gacs.AddRange(gacs);	// Add to _references instead ?

					// Get References
					var unresolvedReferences = CaptureMatches(header, @"reference (?<reference>[\w|\.|\\|\-|:]+);", "reference");
					var resolvedReferences = ResolvePaths(unresolvedReferences);
					_references.AddRange(resolvedReferences);

					// Get Includes
					var unresolvedIncludes = CaptureMatches(header, @"include (?<include>[\w|\.|\\|\-|:]+);", "include");
					var resolvedIncludes = ResolvePaths(unresolvedIncludes); 
					_includes.AddRange(resolvedIncludes);

					// Entry point ?
				}
			}
		}

		private IEnumerable<string> CaptureMatches(string input, string pattern, string capture)
		{
			var matches = Regex.Matches(input, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

			return (from Match match in matches select match.Groups[capture].Value).ToList();
		}

		private IEnumerable<string> ResolvePaths(IEnumerable<string> paths)
		{
			foreach (var path in paths)
			{
				if (PathHelper.IsAbsolutePath(path))
					yield return path;
				else
					yield return PathHelper.ResolveRelativePath(BasePath, path);	// relative
			}
		}
		#endregion
	}
}
