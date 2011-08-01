using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnvDTE;

// Make Add method that takes Entry ?
// Should !scripts.Any() be checked in Add method or Entry constructor ? or not here at all - no logic here - just a dump container

namespace Dynamo.Jiss.AddIn
{
	internal class Index : IIndex
	{
		#region Fields
		private readonly Dictionary<Project, Dictionary<ProjectItem, Entry>> _entries = new Dictionary<Project, Dictionary<ProjectItem, Entry>>();
		#endregion

		#region Methods
		public void Add(ProjectItem item, EventModel model, IEnumerable<IEventScript> scripts, ResolveEventHandler resolver)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var entry = new Entry(item, model, scripts, resolver);

			var project = item.ContainingProject;
	
			Dictionary<ProjectItem, Entry> items;
			if (!_entries.TryGetValue(project, out items))
			{
				// Add project entry
				items = new Dictionary<ProjectItem, Entry>();
				_entries.Add(project, items);
			}

			items.Add(item, entry);
		}
		
		public void Remove(Project project)
		{
			if (project == null)
				throw new ArgumentNullException("project");

			_entries.Remove(project);
		}
		public void Remove(ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var project = item.ContainingProject;

			Dictionary<ProjectItem, Entry> items;
			if (_entries.TryGetValue(project, out items))
			{
				items.Remove(item);
			}
		}

		public bool TryGet(ProjectItem item, out Entry entry)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var project = item.ContainingProject;

			Dictionary<ProjectItem, Entry> items;
			if (_entries.TryGetValue(project, out items))
			{
				return items.TryGetValue(item, out entry);
			}

			entry = null;
			return false;
		}

		public IEnumerable<Entry> GetAll()
		{
			return _entries.Values.SelectMany(value => value.Values);
		}
		public IEnumerable<Entry> GetAll(Project project)
		{
			// TryGetAll - bool - out IEnumerable<Entry> ?

			if (project == null)
				throw new ArgumentNullException("project");

			Dictionary<ProjectItem, Entry> items;
			if (_entries.TryGetValue(project, out items))
			{
				return items.Values;
			}

			return Enumerable.Empty<Entry>();
		}

		public bool Contains(Project project)
		{
			if (project == null)
				throw new ArgumentNullException("project");

			return _entries.ContainsKey(project);
		}

		public bool Contains(ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var project = item.ContainingProject;

			Dictionary<ProjectItem, Entry> items;
			if (_entries.TryGetValue(project, out items))
			{
				return items.ContainsKey(item);
			}

			return false;
		}
		#endregion
	}
}
