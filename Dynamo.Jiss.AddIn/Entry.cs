using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

// Better name ? - Unit ?

namespace Dynamo.Jiss.AddIn
{
	internal class Entry
	{
		private readonly List<IEventScript> _scripts = new List<IEventScript>();

		public Entry(ProjectItem item, EventModel model, IEnumerable<IEventScript> scripts, ResolveEventHandler resolver)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (model == null)
				throw new ArgumentNullException("model");
			if (scripts == null)
				throw new ArgumentNullException("scripts");
			if (resolver == null)
				throw new ArgumentNullException("resolver");

			if (!scripts.Any())
				throw new ArgumentException("Scripts should not be 0.", "scripts");

			ProjectItem = item;
			EventModel = model;

			//Assembly = scripts.First().GetType().Assembly;						// Dont require the Assembly to be passed ?
			
			_scripts.AddRange(scripts);	
			Resolver = resolver;
		}

		public ProjectItem ProjectItem { get; private set; }

		//public Assembly Assembly { get; private set; }							// Not being used ?

		public EventModel EventModel { get; private set; }

		public IEnumerable<IEventScript> Scripts { get { return _scripts; } }		// Not really needed ?

		public ResolveEventHandler Resolver { get; private set; }
	}
}
