using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Dynamo.Jiss.Compilation
{
	public static class CompilerHelper
	{
		public static CompilerParameters CreateParameters(IEnumerable<string> references = null, bool optimize = true, bool includeDebugInformation = false, bool treatWarningsAsErrors = false, int warningLevel = -1)
		{
			var parameters = new CompilerParameters();
			parameters.SetParameters(references, optimize, includeDebugInformation, treatWarningsAsErrors, warningLevel);

			return parameters;
		}

		public static Dictionary<string, string> GetProviderOptions(CompilerVersion version)
		{
			Dictionary<string, string> providerOptions = new Dictionary<string, string>();

			var str = GetCompilerVersion(version);
			if (str != null)
				providerOptions.Add("CompilerVersion", str);

			return providerOptions;
		}

		public static CodeDomProvider CreateProvider(Language language, CompilerVersion version)
		{
			var providerOptions = GetProviderOptions(version);
			var provider = CodeDomProvider.CreateProvider(language.ToString(), providerOptions);

			return provider;
		}

		public static string GetCompilerVersion(CompilerVersion version)
		{
			// attach to enum via description attribute instead... ?

			switch (version)
			{
				case CompilerVersion.v20:
					return "v2.0";
				case CompilerVersion.v30:
					return "v3.0";
				case CompilerVersion.v35:
					return "v3.5";
				case CompilerVersion.v40:
					return "v4.0";
				default:
					return null;
			}
		}
	}
}