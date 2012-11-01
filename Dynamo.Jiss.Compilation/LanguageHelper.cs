using System;

namespace Dynamo.Jiss.Compilation
{
	public static class LanguageHelper
	{
		public static Language GetLanguage(string filename)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");

			if (filename.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
			{
				return Language.CSharp;
			}

			if (filename.EndsWith(".vb", StringComparison.OrdinalIgnoreCase))
			{
				return Language.VisualBasic;
			}

			// Throw exception instead?
			return Language.None;
		}
	}
}
