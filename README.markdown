
WHAT IS IT
----------
C# as a "Scripting" Language, including an Add-in for Visual Studio that makes it possible to execute scripts on Visual Studio events.



HOW
---------
Install the add-in (put the files in the add-in folder) and enable it in Visual Studio.
Reference the Dynamo.Jiss assembly for intellisense (no need to copy it to output dir), create a file with the jiss.cs extension (no need to compile it) and implement the IEventScript interface to setup the events.

Currently it supports the following header directives (must be put as the first thing in the file and start with //):

// gac System.dll;
// reference c:\MyAssembly.dll;		(could be a relative path to the script file)
// include c:\MyFile.cs;			(could be a relative path to the script file)

There is several helpers in the Dynamo.Jiss namespace to solve trivial tasks.

I use it for combining files and minifying javascript for an example.

More to come later ...



DISCLAIMER
-----------
This is a _very_ early prototype, and i am just playing around with it. 
