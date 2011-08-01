using System;
using EnvDTE;

namespace Dynamo.Jiss.AddIn
{
	internal interface ICore : IDisposable
	{
		void Load(Solution solution);
		void Load(Project project);
		void Load(ProjectItem item);

		void Unload(Project project);
		void Unload(ProjectItem item);

		void Execute(Project project, Action<EventModel> action);
	}
}
