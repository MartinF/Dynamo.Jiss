using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Dynamo.Jiss.Compilation
{
	public static class CompilerExtensions
	{
		public static void SetParameters(this CompilerParameters parameters, IEnumerable<string> references = null, bool optimize = true, bool includeDebugInformation = false, bool treatWarningsAsErrors = false, int warningLevel = -1)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			if (references != null)
				parameters.ReferencedAssemblies.AddRange(references.ToArray());

			if (optimize)
				parameters.CompilerOptions = "/optimize";

			parameters.IncludeDebugInformation = includeDebugInformation;
			parameters.TreatWarningsAsErrors = treatWarningsAsErrors;
			parameters.WarningLevel = warningLevel;
		}

		public static void SetGenerateInMemory(this CompilerParameters parameters)
		{
			// Set Generation Mode / Type ?

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			//parameters.MainClass = "";	// option together with generateExecutable ?
			
			parameters.GenerateExecutable = false;
			parameters.GenerateInMemory = true;
		}
	}
}
