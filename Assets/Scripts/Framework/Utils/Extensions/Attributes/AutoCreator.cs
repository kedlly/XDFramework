using System;

namespace Framework.Utils.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PathInHierarchyAttribute : Attribute
	{
		public string Path { get; private set; }
		public PathInHierarchyAttribute(string pathInHierarchy)
		{
			Path = pathInHierarchy;
			if (Path == null)
			{
				Path = "";
			}
		}
		public PathInHierarchyAttribute()
		{
			Path = "";
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class DontDestroyOnLoadAttribute : Attribute
	{

	}
}
