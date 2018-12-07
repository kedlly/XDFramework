


using XDDQFrameWork.Editor.ProjectBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public static class CommandLineFunction
{
	//command line function
	private static void PreBuildProcess()
	{
		string[] commandLine = Environment.GetCommandLineArgs();
		int indexOfThisCommand = -1;
		for (int i = 0; i < commandLine.Length; ++i )
		{
			if ( commandLine[i] == "CommandLineFunction.PreBuildProcess" )
			{
				indexOfThisCommand = i;
				break;
			}
		}
		if ( indexOfThisCommand != -1)
		{
			int argFileIndex = indexOfThisCommand + 1;
			string fileName = commandLine[argFileIndex];
			string filePath = DataCollection.GetFilePathDefault(fileName);
			DataCollection.ImportBuildSettingFromXML(filePath);
		}
	}

	private static string GetArgArray(string [] args, int startIndex, int EndIndex, string tag)
	{
		for (int i = startIndex;i < EndIndex; ++i )
		{
			if (args[i].StartsWith(tag, StringComparison.OrdinalIgnoreCase))
			{
				var argValue = args[i].Split('=');
				return argValue[1];
			}
		}
		return null;
	}
	/*
	private static void Build()
	{
		string[] commandLine = Environment.GetCommandLineArgs();
		DebugUtil.Log(string.Join("|", commandLine));
		int indexOfThisCommand = -1;
		for ( int i = 0; i < commandLine.Length; ++i )
		{
			if ( commandLine[i] == "CommandLineFunction.Build" )
			{
				indexOfThisCommand = i;
				break;
			}
		}
		if ( indexOfThisCommand != -1 )
		{
			int argIndex = indexOfThisCommand + 1;
			string cfgFileName = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.cfg");
			string outDir = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.outDir");
			//string groupStr = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.group");
			string targetStr = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.target");
			// 1.step
			DataCollection.ImportBuildSettingFromXML(DataCollection.GetFilePathDefault(cfgFileName));
			// 2.step
			BuildTarget target = BuildTarget.Android;
			targetStr = targetStr.ToLower();
			if ( targetStr == "android")
			{
				target = BuildTarget.Android;
			}
			else if (targetStr == "ios")
			{
				target = BuildTarget.iOS;
			}
			else
			{
				throw new Exception("目前只支持ios和android平台打包.");
			}

			DataCollection.ReloadBuildSettings(target);
			
			try
			{
				GenericBuild(FindEnabledEditorScenes()
						, outDir
						, BuildPipeline.GetBuildTargetGroup(target)
						, target
						, BuildOptions.AcceptExternalModificationsToPlayer
					);
			}
			catch ( Exception ex )
			{
				throw ex;
			}
		}
	}*/

	private static void Build()
	{
		string[] commandLine = Environment.GetCommandLineArgs();
		UnityEngine.Debug.Log(string.Join("|", commandLine));
		int indexOfThisCommand = -1;
		for ( int i = 0; i < commandLine.Length; ++i )
		{
			if ( commandLine[i] == "CommandLineFunction.Build" )
			{
				indexOfThisCommand = i;
				break;
			}
		}
		if ( indexOfThisCommand != -1 )
		{
			int argIndex = indexOfThisCommand + 1;
			string cfgPackageName = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.cfgPackage");
			string outDir = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.outDir");
			//string groupStr = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.group");
			string targetStr = GetArgArray(commandLine, argIndex, commandLine.Length, "clf.target");
			// 1.step
			DataCollection.ImportBuildSettingFromXML(DataCollection.GetPackageTargetXML(cfgPackageName));
			// 2.step
			BuildTarget target = BuildTarget.Android;
			targetStr = targetStr.ToLower();
			if ( targetStr == "android" )
			{
				target = BuildTarget.Android;
			}
			else if ( targetStr == "ios" )
			{
				target = BuildTarget.iOS;
			}
			else
			{
				throw new Exception("目前只支持ios和android平台打包.");
			}

			DataCollection.ReloadBuildSettings(target);

			try
			{
				System.IO.Directory.CreateDirectory(outDir);
				GenericBuild(FindEnabledEditorScenes()
						, outDir
						, BuildPipeline.GetBuildTargetGroup(target)
						, target
						, BuildOptions.AcceptExternalModificationsToPlayer
					);
			}
			catch ( Exception ex )
			{
				throw ex;
			}
		}
	}


	private static void RunNextScript(string scriptName)
	{
		Process proc = new Process();
		proc.StartInfo.FileName = scriptName;
		proc.Start();
	}


	private static string[] FindEnabledEditorScenes()
	{
		List<string> EditorScenes = new List<string>();
		foreach ( EditorBuildSettingsScene scene in EditorBuildSettings.scenes )
		{
			if ( !scene.enabled )
				continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}

	private static void GenericBuild(string[] scenes, string target_dir, BuildTargetGroup build_group,BuildTarget build_target, BuildOptions build_options)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(build_group, build_target);
#if UNITY_2018
		var buildReport = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
		UnityEngine.Debug.Log(buildReport);
#else
		string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

		if ( res.Length > 0 )
		{
            string strScenes = "(";
            foreach(var val in scenes)
            {
                strScenes += val + ",";
            }
            strScenes += ")";

            string optionsInfo = "scenes:" + strScenes
                + "|target_dir:" + target_dir 
                + "|build_group:" + build_group.ToString() 
                + "|build_target:" + build_target.ToString()
                + "|build_options:" + build_options.ToString()
                ;

			throw new Exception("BuildPlayer failure: " + res + "***optionsInfo->" + optionsInfo);
		}
#endif
	}

}

