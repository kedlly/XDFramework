using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace Assets.Editor.ProjectBuilder
{
	public interface IProjectBuilder : IPostprocessBuild, IPreprocessBuild
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
