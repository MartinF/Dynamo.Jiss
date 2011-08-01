using System;

namespace Dynamo.Jiss.Compilation
{
	public static class LanguageHelper
	{
		public static Language GetLanguage(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			if (name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
			{
				return Language.CSharp;
			}

			if (name.EndsWith(".vb", StringComparison.OrdinalIgnoreCase))
			{
				return Language.VisualBasic;
			}

			return Language.None;
		}
	}
}
