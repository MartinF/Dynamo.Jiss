using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dynamo.Jiss.Compilation;
using EnvDTE;

// Load assemblies into seperate AppDomain to be able to unload them later.

// Implement IReporter / IFeedbackManager for handling feedback ? MessageBox, Output window, Error list?

// Should Events in EventModel automatically include Path of file relative to Project base path and other usefull stuff ? etc cancel of build ?

// Add more events ?

// How to handle the Resolving of the assemblies created and their references ? Global or for each assembly generated?

// Write tests, helpers, extensions, core, compilation ... 

namespace Dynamo.Jiss.AddIn
{
	internal class Core : ICore
	{
		#region Fields
		private readonly IIndex _index;
		#endregion

		#region Constructors
		public Core(IIndex index)
		{
			if (index == null)
				throw new ArgumentNullException("index");

			_index = index;		
		}
		public Core() : this(new Index())
		{
			// Uses default implementation of IIndex
		}
		#endregion

		#region Methods
		private void Report(string text)
		{
			MessageBox.Show(Settings.AppName + " reporting:" + Environment.NewLine + text);
		}

		public void Load(Solution solution)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");

			foreach (Project project in solution)
			{
				Load(project);
			}
		}
		public void Load(Project project)
		{
			if (project == null)
				throw new ArgumentNullException("project");

			// Do not load if Project Soulitoon Items or Project Miscellaneous Items
			// What about vsProjectKindUnmodeled ? or other types ?
			// Better to move the check to Load(ProjectItem item) - and check if it is a Jiss file, or just IsProjectFile ?
			if (project.Kind == Constants.vsProjectKindSolutionItems || project.Kind == Constants.vsProjectKindMisc)
				return;

			// Find all scripts in the project
			var items = project.GetAllItems().Where(x => x.IsJiss());

			// Try to load them
			foreach (var item in items)
			{
				Load(item);
			}
		}
		public void Load(ProjectItem item)
		{
			// Split the logic of this method into smaller pieces
			// Wrap with Try/Catch and print any exceptions (if debug build etc)
			// Handle missing references/includes better - print out what is being loaded and if some path is not found

			

			if (item == null)
				throw new ArgumentNullException("item");
			
			// If this is public then check if it is a Jiss file here ... else make private ?
			// Right now every method that calls it checks it instead ?

			// Check if item is ProjectFile or IsJiss here ?
			//if (!item.IsProjectFile() or IsJiss())
			//    return;



			var filename = item.FileNames[0];	// create extension method GetFullName ?

			// Load the file and create a reader
			JissFile script;
			IJissReader reader;
			try
			{
				script = JissFile.Create(filename);
				reader = new JissReader(script);		// Should reader take in filename and store a target property to get Language etc ? or just TargetLanguage property?
			}
			catch (Exception)
			{
				Report("Error occured when trying to load file: " + filename);		// Could be one of the includes - better error handling
				return;
			}

			// Create Provider/Compiler
			CompilerResults result;
			using (var provider = CompilerHelper.CreateProvider(script.Language, CompilerVersion.v40))
			{
				// Handle references
				var references = Helper.GetDefaultReferences().ToList();
				references.AddRange(reader.GetGacs());
				references.AddRange(reader.GetReferences());

				// Create parameters
				var parameters = CompilerHelper.CreateParameters(references, true, false, false);
				parameters.SetGenerateInMemory();

				// Compile
				result = provider.CompileAssemblyFromSource(parameters, reader.GetSources().ToArray());
			}

			// What about warnings ?
			if (result.Errors.HasErrors)
			{
				string errors = "";

				foreach (CompilerError error in result.Errors)
				{
					errors += error.ErrorText + Environment.NewLine;
				}

				Report(errors);	// Show errors in the Error List instead !?
				
				return;
			}



			// Create the resolver for the referenced assemblies

			var assemblyReferences = result.CompiledAssembly.GetReferencedAssemblies();
			var readerReferences = reader.GetReferences();

			Dictionary<string, string> assemblyNames = new Dictionary<string, string>();	// Contains AssemblyName.FullName / Full reference
			foreach (var reference in readerReferences)
			{
				var assName = AssemblyName.GetAssemblyName(reference);

				foreach (var assReference in assemblyReferences)
				{
					if (assName.FullName == assReference.FullName)
						assemblyNames.Add(assName.FullName, reference);
				}
			}

			var jissAssName = typeof(IEventScript).Assembly.FullName;	// add to assemblyNames + reference ?
			var resolver = new ResolveEventHandler((obj, args) =>
												   {
													   if (args.RequestingAssembly == result.CompiledAssembly)
													   {
														   string fullReference;
														   if (assemblyNames.TryGetValue(args.Name, out fullReference))
															   return Assembly.LoadFrom(fullReference);

														   // Move this into seperate global resolver ?
														   if (args.Name == jissAssName)
															   return Assembly.Load(args.Name);	// use AssemblyName instead ?
													   }

													   return null;
												   });

			// Only attach (and create) resolver if readerReferences.Any() ? requires seperate resolver for Dynamo.Jiss assembly then...
			AppDomain.CurrentDomain.AssemblyResolve += resolver;



			// Get all types/classes which implement the IEventScript interface
			var types = result.CompiledAssembly.GetTypesAssignableTo<IEventScript>();

			// Create an instance for each type found and call setup
			var eventModel = new EventModel();
			var instances = new List<IEventScript>();
			foreach (var type in types)
			{
				IEventScript instance;
				try
				{
					instance = (IEventScript)Activator.CreateInstance(type);
				}
				catch (Exception)
				{
					Report("Error occured when trying to create an instance of type: " + type.Name + Environment.NewLine + "Please check that the type have an empty public constructor.");
					continue;
				}

				try
				{
					instance.Setup(item, eventModel);
				}
				catch (Exception)
				{
					Report("Error occured in the Setup() method for type: " + type.Name);
					continue;
				}

				instances.Add(instance);
			}

			// Add to index if any instances was created
			if (instances.Any())
			{
				_index.Add(item, eventModel, instances, resolver);
			}
			else
			{
				AppDomain.CurrentDomain.AssemblyResolve -= resolver;
			}
		}

		public void Unload(Project project)
		{
			if (project == null)
				throw new ArgumentNullException("project");

			foreach (var entry in _index.GetAll(project))
				AppDomain.CurrentDomain.AssemblyResolve -= entry.Resolver;
			
			_index.Remove(project);
		}
		public void Unload(ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			// Get item and remove resolver
			Entry entry;
			if (_index.TryGet(item, out entry))
				AppDomain.CurrentDomain.AssemblyResolve -= entry.Resolver;

			_index.Remove(item);
		}

		public void Execute(Project project, Action<EventModel> action)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (action == null)
				throw new ArgumentNullException("action");

			// Enumerate all scripts within the project and execute the action on the EventModel
			foreach (var entry in _index.GetAll(project))
			{
				try
				{
					action(entry.EventModel);
				}
				catch (Exception e)
				{
					var errorMsg = "The " + action.Method.Name + " event in " + entry.ProjectItem.Name + " threw an exception.";
					errorMsg += Environment.NewLine + "Stack trace: " + e.StackTrace;
					Report(errorMsg);
				}
			}
		}

		public void Dispose()
		{
			foreach (var entry in _index.GetAll())
			{
				AppDomain.CurrentDomain.AssemblyResolve -= entry.Resolver;
			}
		}
		#endregion
	}
}





// Old AssemblyResolve methods
// ---------------------------


// Load all references assemblies immediatly ?

// Could do it the other way around and look for references in readerReferences that endsWith() name + .dll ?
//foreach (var reference in readerReferences)
//{
//    var assName = AssemblyName.GetAssemblyName(reference);

//    foreach (var assReference in assemblyReferences)
//    {
//        if (assName.FullName == assReference.FullName)
//            Assembly.LoadFrom(reference);
//    }
//}




//public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
//{
//    if (args.RequestingAssembly != null && args.RequestingAssembly.Location == string.Empty && !args.RequestingAssembly.GlobalAssemblyCache)
//    {
//        // Problem is, that it can get here even when it is not the compiled assembly - etc other addins which will probably throw exception

//        Assembly ass;
//        try
//        {
//            ass = Assembly.Load(args.Name);
//            return ass;
//        }
//        catch (Exception)
//        {
//        }
//    }

//    return null;
//}

//public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
//{
//    // Only handle under these conditions
//    // Really only need to check RequestingAssembly == null TryGetValue
//    IEnumerable<string> refs;
//    if (args.RequestingAssembly != null && args.RequestingAssembly.Location == string.Empty && !args.RequestingAssembly.GlobalAssemblyCache)// && _assemblies.TryGetValue(args.RequestingAssembly, out refs))
//    {
//        // Check if it is Dynamo.Jiss?
//        //if (args.Name == typeof(IEventScript).Assembly.FullName)	// Cache it to static variables ?
//        //    return typeof(IEventScript).Assembly;

//        // OR

//        Assembly ass;
//        try
//        {
//            ass = Assembly.Load(args.Name);	// Load all using LoadFrom and then only attach this ? and remove the static dictionary ?
//            return ass;
//        }
//        catch (Exception)
//        {
//        }

//        //foreach (var assemblyName in args.RequestingAssembly.GetReferencedAssemblies())		// Cache GetReferenencedAssemblies() ?
//        //{
//        //    if (args.Name == assemblyName.FullName)
//        //    {
//        //        foreach (var fullReference in refs)
//        //        {
//        //            if (fullReference.EndsWith(assemblyName.Name + ".dll"))		// poor - use contains ? - could be .exe ?
//        //            {
//        //                try
//        //                {
//        //                    var assName = AssemblyName.GetAssemblyName(fullReference);
//        //                    if (assName.FullName == args.Name)
//        //                    {
//        //                        ass = Assembly.LoadFrom(fullReference);
//        //                        return ass;
//        //                    }
//        //                }
//        //                catch (Exception)
//        //                {
//        //                }
//        //            }
//        //        }
//        //    }
//        //}
//    }

//    return null;
//}