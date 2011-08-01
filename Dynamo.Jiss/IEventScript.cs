using EnvDTE;

// IFeedbackManager which contains different methods for making it easy to use either output window, error list or messagebox ?

// Project or ProjectItem ?
// Wrap data in some type and include Application/Solution ?

// Script Dir, and Project Dir for making it easy to write files and such inside a project ?

namespace Dynamo.Jiss
{
	public interface IEventScript
	{
		void Setup(ProjectItem item, EventModel events);
	}
}
