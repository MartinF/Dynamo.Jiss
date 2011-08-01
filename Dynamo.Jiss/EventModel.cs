using EnvDTE;

// Use Visual Studio eventhandlers ? or create customs ones myself and add extra information (etc relative project path) ?
// Also inject a IFeedback/IReporter interface for communicating with output window or error list etc ?
// Could make it possible to easy stop build in OnProjectBuildConfigBegin etc ? 

namespace Dynamo.Jiss
{
	public class EventModel
	{
		// Events
		public event _dispProjectItemsEvents_ItemAddedEventHandler OnProjectItemAdded;
		public event _dispProjectItemsEvents_ItemRemovedEventHandler OnProjectItemRemoved;
		public event _dispProjectItemsEvents_ItemRenamedEventHandler OnProjectItemRenamed;
		public event _dispDocumentEvents_DocumentSavedEventHandler OnDocumentSaved;
		public event _dispBuildEvents_OnBuildProjConfigBeginEventHandler OnProjectBuildConfigBegin;

		// Methods - executing the events

		public void ProjectItemAdded(ProjectItem item)
		{
			if (OnProjectItemAdded != null)
				OnProjectItemAdded(item);
		}

		public void ProjectItemRemoved(ProjectItem item)
		{
			if (OnProjectItemRemoved != null)
				OnProjectItemRemoved(item);
		}

		public void ProjectItemRenamed(ProjectItem item, string oldName)
		{
			if (OnProjectItemRenamed != null)
				OnProjectItemRenamed(item, oldName);
		}

		public void DocumentSaved(Document document)
		{
			if (OnDocumentSaved != null)
				OnDocumentSaved(document);
		}
	
		public void ProjectBuildConfigBegin(string projectName, string projectConfig, string platform, string solutionConfig)
		{
			if (OnProjectBuildConfigBegin != null)
				OnProjectBuildConfigBegin(projectName, projectConfig, platform, solutionConfig);
		}


		
		// Events
		//public event DocumentSavedDelegate OnDocumentSaved;
		//public event BuildProjectConfigurationBeginDelegate OnBuildProjectConfigurationBegin;

		// Delegates
		//public delegate void DocumentSavedDelegate(Document document);
		//public delegate void BuildProjectConfigurationBeginDelegate(string projectName, string projectConfig, string platform, string solutionConfig);
	}
}