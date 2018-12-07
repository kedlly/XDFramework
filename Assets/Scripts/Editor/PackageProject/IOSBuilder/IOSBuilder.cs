using System;
using UnityEditor.Build;
using UnityEditor;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace XDDQFrameWork.Editor.ProjectBuilder
{
	public class IOSBuilder : IProjectBuilder
	{
		int IOrderedCallback.callbackOrder { get { return 0; } }

#if !UNITY_2018_1_OR_NEWER
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
			//throw new NotImplementedException();
		}

		void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
		{
			//throw new NotImplementedException();
		}
#endif
	}
}
