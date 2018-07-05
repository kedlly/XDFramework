using System;
using UnityEditor.Build;
using UnityEditor;

namespace Assets.Editor.ProjectBuilder
{
	public class AndroidBuilder : IProjectBuilder
	{
		int IOrderedCallback.callbackOrder { get { return 0; } }

		void IPostprocessBuild.OnPostprocessBuild(BuildTarget target, string path)
		{
			//throw new NotImplementedException();
		}

		void IPreprocessBuild.OnPreprocessBuild(BuildTarget target, string path)
		{
			//throw new NotImplementedException();
		}
	}
}