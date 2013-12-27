using System;
using System.Collections.Generic;
using System.Linq;

// Reads an JissScript and provides information for the entire script - including gacs references and includes.

// Could make sure all JissScripts loaded are the same language as target ?
// Make possible to specify file ? make static helper etc ? - should expose Language for target file somehow ?

namespace Dynamo.Jiss.Compilation
{
	public class JissReader : IJissReader
	{
		#region Fields
		private readonly List<JissBase> _scripts = new List<JissBase>();
		#endregion

		#region Constructors
		public JissReader(JissFile target)
		{
			if (target == null)
				throw new ArgumentNullException("target");

			_scripts.Add(target);			// Happens in both constructors - could be first part of ReadIncludes instead somehow ? or create private constructor ?

			var loaded = new List<string>();
			loaded.Add(target.FullName);

			ReadIncludes(target, loaded);
		}
		public JissReader(JissScript target)
		{
			if (target == null)
				throw new ArgumentNullException("target");

			_scripts.Add(target);

			var loaded = new List<string>();

			ReadIncludes(target, loaded);
		}
		#endregion

		#region
		private void ReadIncludes(JissBase script, List<string> loaded)
		{
			foreach (var include in script.GetIncludes())
			{
				// If not already loaded - load it - 
				// throw exception if trying to load one that is already loaded ? Circular dependency !?
				if (!loaded.Contains(include))
				{
					// Read it
					var jiss = JissFile.Create(include);
					
					// Save it
					loaded.Add(include);
					_scripts.Add(jiss);

					// Read the includes
					ReadIncludes(jiss, loaded);
				}
			}
		}

		public IEnumerable<string> GetSources()
		{
			return _scripts.Select(jiss => jiss.Source);
		}

		public IEnumerable<string> GetReferences()
		{
			// Filter duplicates ?

			return _scripts.SelectMany(jiss => jiss.GetReferences());
		}

		public IEnumerable<string> GetGacs()
		{
			// Filter duplicates ?

			return _scripts.SelectMany(jiss => jiss.GetGacs());
		}
		#endregion
	}
}
