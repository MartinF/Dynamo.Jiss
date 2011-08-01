using System.IO;

namespace Dynamo.Jiss
{
	public interface IExecutableScript
	{
		// Script Fullname or/and DirectoryRoot ? Why not just pass FileInfo ?

		// Do not use FileInfo, have a lot of properties for handling the file which shouldnt be used for anything
		// on the other hand it is also easy to use for getting path and so on..

		void Execute(FileInfo fileInfo);	// <- string[] args or use static main for that always ? then no Interface or entry point have to specified
	}
}
