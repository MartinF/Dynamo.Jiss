using System;
using System.CodeDom.Compiler;
using System.Linq;
using Dynamo.Jiss.Compilation;
using Console = System.Console;

// No implementation yet - currently just used for some quick testing

// If there is no types with interface IExecutableScript run entry point - Static Main() ? or if entry point defined ?
// If entry point not specified assume static Main ? if specified assume instance method ? or always expect instance method?

namespace Dynamo.Jiss.ConsoleChangeThisNamespaceItisBegingAnoyingShouldNotBeConsole // FIX IT !
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//// Trying with relative path
			//references.Add(@"C:\Users\Martin\Projects\Dynamo.Jiss\Dynamo.Jiss.Console\bin\..\bin\Debug\Dynamo.Jiss.dll");	// would this work ?



			var targetFilename = @"C:\Users\Martin\Desktop\Mess\TestMvcApplication\TestMvcApplication\Whatever.jiss.cs";

			var jiss = JissFile.Create(targetFilename);
			var reader = new JissReader(jiss);
			var test1 = reader.GetReferences();
			var test2 = reader.GetSources();

			CompilerResults result;
			using (var provider = CompilerHelper.CreateProvider(jiss.Language, CompilerVersion.v40))
			{
				// Handle references
				var references = ReferenceHelper.GetFrameworkReferences().ToList();
				//references.AddRange(ReferenceHelper.GetVisualStudioReferences());
				references.Add(typeof(IEventScript).Assembly.Location);

				references.AddRange(reader.GetReferences());	// ?

				// Create helper that takes parameters in when creating the provider ?
				var parameters = CompilerHelper.CreateParameters(references.ToArray());	// take in IEnumerable<string> ?
				parameters.SetGenerateInMemory();
				
				result = provider.CompileAssemblyFromSource(parameters, reader.GetSources().ToArray());	// Read all the sources output as string[] or IEnumerable<string> !
			}

			if (result.Errors.HasErrors)
			{
				string errors = "";

				foreach (CompilerError error in result.Errors)
				{
					// Show errors in the Error List !???
					errors += error.ErrorText + Environment.NewLine; // Include more options - should show in ErrorList?
				}

				Console.WriteLine(errors);
				return;
			}



			//var ref1 = result.CompiledAssembly.GetReferencedAssemblies();
			//var what2 = Assembly.GetEntryAssembly();



			var types = result.CompiledAssembly.GetTypesAssignableTo<IEventScript>();
			foreach (var type in types)
			{
				var instance = (IEventScript)Activator.CreateInstance(type);
				instance.Setup(null, null);
			}



			System.Console.ReadLine();
		}
	}
}
