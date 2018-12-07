using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace XDDQFrameWork.Editor.ProjectBuilder
{
	public interface IProjectBuilder
#if !UNITY_2018
	:IPostprocessBuild, IPreprocessBuild
#else
	:IPostprocessBuildWithReport, IPreprocessBuildWithReport
#endif
	{
	}


	public class BuilderFactory //: ToSingleton<BuilderFactory>
	{
		private IProjectBuilder builderItf;
		private BuilderFactory()
		{

		}

		public void SetTargetBuilder(IProjectBuilder itf)
		{
			this.builderItf = itf;
		}

		public void Build()
		{
			if (this.builderItf != null)
			{
				//this.builderItf.OnPostprocessBuild()
			}
		}

		public void PreBuild(BuildTarget target, string pathToBuiltProject)
		{

		}

		public void Build(BuildTarget target, string buildPath)
		{
			switch(target)
			{
			case BuildTarget.Android:
				{

				}
				break;
			case BuildTarget.iOS:
				{

				}
				break;
			default:
				break;
			}
		}
	}
}
