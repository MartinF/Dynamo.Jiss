using System;
using System.Collections.Generic;
using EnvDTE;

namespace Dynamo.Jiss.AddIn
{
	internal interface IIndex
	{
		void Add(ProjectItem item, EventModel model, IEnumerable<IEventScript> scripts, ResolveEventHandler resolver);

		void Remove(Project project);
		void Remove(ProjectItem item);

		bool TryGet(ProjectItem item, out Entry entry);

		IEnumerable<Entry> GetAll();
		IEnumerable<Entry> GetAll(Project project);

		bool Contains(Project project);
		bool Contains(ProjectItem item);
	}
}