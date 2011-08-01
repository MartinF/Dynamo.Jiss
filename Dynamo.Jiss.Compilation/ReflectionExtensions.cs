using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Constraints ?

namespace Dynamo.Jiss.Compilation
{
	public static class ReflectionExtensions
	{
		public static IEnumerable<Type> GetTypesAssignableTo<T>(this Assembly assembly)
		{
			return assembly.GetTypes().Where(type => type.IsClass && typeof(T).IsAssignableFrom(type));
		}
	}
}