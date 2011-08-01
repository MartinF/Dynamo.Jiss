using System.Collections.Generic;

namespace Dynamo.Jiss.Compilation
{
	public interface IJissReader
	{
		IEnumerable<string> GetSources();
		IEnumerable<string> GetReferences();
		IEnumerable<string> GetGacs();
	}
}
