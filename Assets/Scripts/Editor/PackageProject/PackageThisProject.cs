using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

namespace Assets.Editor.ProjectBuilder
{ 
    public static partial class BuildFunctions {

        //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
        public static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }
            return names.ToArray();
        }

		

		private static void OnPreprocessBuild( BuildTarget target, string pathToBuiltProject)
		{
			//BuilderFactory.Instance.PreBuild(target, pathToBuiltProject);
		}

		private static void BuildForIOS()
		{
			//OnPreprocessBuild(BuildTarget.iOS, pathToBuiltProject);
			Debug.Log(System.Environment.CommandLine);
			System.IO.File.WriteAllText(@"C:\Users\kedllyzhao\Desktop\1.txt", System.Environment.CommandLine);
		}

		private static void BuildForAndroid()
		{
			OnPreprocessBuild( BuildTarget.iOS, "");
			System.IO.File.WriteAllText(@"C:\Users\kedllyzhao\Desktop\2.txt", "adsfasdfad");
		}

		private static void BuildForWin32()
		{
			OnPreprocessBuild(BuildTarget.StandaloneWindows, "");
		}

		private static void BuildForWin64()
		{
			OnPreprocessBuild(BuildTarget.StandaloneWindows64, "");
		}

		[PostProcessBuild(999)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			//BuilderFactory.Instance.Build(target, pathToBuiltProject);
			if(target == BuildTarget.iOS)
			{
				string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
#if UNITY_IPHONE
                UnityEditor.iOS.Xcode.PBXProject pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
                pbxProject.ReadFromFile(projectPath);
                var projTarget = pbxProject.TargetGuidByName("Unity-iPhone");            
                pbxProject.SetBuildProperty(projTarget, "ENABLE_BITCODE", "NO");
                pbxProject.WriteToFile (projectPath);
#endif            
            }
        }

		[DidReloadScripts]
        public static void drs()
        {
			Debug.Log("All script Reload !");
        }
		
		public class MyAssetHandler
		{
			[OnOpenAssetAttribute(1)]
			public static bool step1(int instanceID, int line)
			{
				string name = EditorUtility.InstanceIDToObject(instanceID).name;
				var obj = EditorUtility.InstanceIDToObject(instanceID);
				UnityEngine.Debug.Log("Open Asset step: 1 (" + name + ")");
				return false; // we did not handle the open
			}

			// step2 has an attribute with index 2, so will be called after step1
			[OnOpenAssetAttribute(2)]
			public static bool step2(int instanceID, int line)
			{
				UnityEngine.Debug.Log("Open Asset step: 2 (" + instanceID + ")");
				return false; // we did not handle the open
			}
		}
		
	}
}