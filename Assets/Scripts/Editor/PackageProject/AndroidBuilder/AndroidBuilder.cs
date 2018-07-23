using System;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Assets.Editor.ProjectBuilder
{
	public class AndroidBuilder : IProjectBuilder
	{
		int IOrderedCallback.callbackOrder { get { return 0; } }


#if !UNITY_2018
		void IPostprocessBuild.OnPostprocessBuild(BuildTarget target, string path)
		{
			//throw new NotImplementedException();
		}


		void IPreprocessBuild.OnPreprocessBuild(BuildTarget target, string path)
		{
			//throw new NotImplementedException();
		}
#else
		void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
		{
			throw new NotImplementedException();
		}

		void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
		{
			throw new NotImplementedException();
		}
#endif
	}
}