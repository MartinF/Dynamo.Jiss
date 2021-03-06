using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace Dynamo.Jiss.AddIn
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2
	{
		#region Fields
		private DTE2 _application;
		private EnvDTE.AddIn _addIn;

		private ICore _core;

		private Events2 _events;
		private ProjectsEvents _projectsEvents;
		private ProjectItemsEvents _projectItemsEvents;
		private DocumentEvents _documentEvents;
		private BuildEvents _buildEvents;
		#endregion

		#region Constructors
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}
		#endregion

		#region Methods
		private void AttachEvents()
		{
			_projectsEvents.ItemAdded += new _dispProjectsEvents_ItemAddedEventHandler(ProjectsEvents_ItemAdded);
			_projectsEvents.ItemRemoved += new _dispProjectsEvents_ItemRemovedEventHandler(ProjectsEvents_ItemRemoved);

			_projectItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(ProjectItemsEvents_ItemAdded);
			_projectItemsEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(ProjectItemsEvents_ItemRemoved);
			_projectItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(ProjectItemsEvents_ItemRenamed);

			_documentEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(DocumentEvents_DocumentSaved);

			_buildEvents.OnBuildProjConfigBegin += new _dispBuildEvents_OnBuildProjConfigBeginEventHandler(BuildEvents_OnBuildProjConfigBegin);
		}

		private void DetachEvents()
		{
			_projectsEvents.ItemAdded -= ProjectsEvents_ItemAdded;
			_projectsEvents.ItemRemoved -= ProjectsEvents_ItemRemoved;

			_projectItemsEvents.ItemAdded -= ProjectItemsEvents_ItemAdded;
			_projectItemsEvents.ItemRemoved -= ProjectItemsEvents_ItemRemoved;
			_projectItemsEvents.ItemRenamed -= ProjectItemsEvents_ItemRenamed;

			_documentEvents.DocumentSaved -= DocumentEvents_DocumentSaved;

			_buildEvents.OnBuildProjConfigBegin -= BuildEvents_OnBuildProjConfigBegin;
		}

		#region VS Hooks
		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_application = (DTE2)application;
			_addIn = (EnvDTE.AddIn)addInInst;

			// Events
			_events = (Events2)_application.Events;
			_projectsEvents = _events.ProjectsEvents;
			_projectItemsEvents = _events.ProjectItemsEvents;
			_documentEvents = _events.DocumentEvents;
			_buildEvents = _events.BuildEvents;

			AttachEvents();
			
			_core = new Core();
			
			// If being connected when Solution is already loaded - try to load all projects
			if (_application.Solution != null)
			{
				_core.Load(_application.Solution);
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
			DetachEvents();

			_core.Dispose();
			_core = null;
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		#endregion

		#region Eventhandlers

		// PROJECT

		void ProjectsEvents_ItemAdded(Project project)
		{
			_core.Load(project);
		}

		void ProjectsEvents_ItemRemoved(Project project)
		{
			_core.Unload(project);
		}

		// PROJECT ITEMS

		void ProjectItemsEvents_ItemAdded(ProjectItem projectItem)
		{
			if (projectItem.IsJiss())
			{
				_core.Load(projectItem);
			}

			// Execute event for all or only if it is a Project Item File ?

			var project = projectItem.ContainingProject;
			_core.Execute(project, x => x.ProjectItemAdded(projectItem));
		}

		void ProjectItemsEvents_ItemRemoved(ProjectItem projectItem)
		{
			if (projectItem.IsJiss())
			{
				_core.Unload(projectItem);
			}

			// Execute event for all or only if is Project Item File ?

			var project = projectItem.ContainingProject;
			_core.Execute(project, x => x.ProjectItemRemoved(projectItem));
		}

		void ProjectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
		{
			if (projectItem.WasJiss(oldName))
			{
				// renamed from jiss file to something else (could still be jiss file but with another name)
				_core.Unload(projectItem);
			}

			if (projectItem.IsJiss())
			{
				// If renamed to jiss file
				_core.Load(projectItem);
			}

			// What if it WasJiss ? should it then execute before Unload ? The jiss file being unloaded could be listning to rename of it self ? ... who cares ...

			var project = projectItem.ContainingProject;
			_core.Execute(project, x => x.ProjectItemRenamed(projectItem, oldName));
		}

		// DOCUMENT

		void DocumentEvents_DocumentSaved(Document document)
		{
			if (document.ProjectItem.IsJiss())
			{
				// Unload and load again
				_core.Unload(document.ProjectItem);
				_core.Load(document.ProjectItem);
			}

			// Always or only if not IsJiss ?

			var project = document.ProjectItem.ContainingProject;
			_core.Execute(project, x => x.DocumentSaved(document));
		}

		// BUILD

		void BuildEvents_OnBuildProjConfigBegin(string projectName, string projectConfig, string platform, string solutionConfig)
		{
			// Get Project
			var project = _application.Solution.Item(projectName);

			// Execute all that listens to this event
			_core.Execute(project, x => x.ProjectBuildConfigBegin(projectName, projectConfig, platform, solutionConfig));
		}

		#endregion

		#endregion
	}
}