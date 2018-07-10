using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Editor.ProjectBuilder;
using UnityEngine;
using UnityEditor;

namespace Plugin.PackageProject
{
    public static class XDProjectMenu
	{

		[MenuItem("XDProject/ExportProjectSettings")]
		private static void ExportProjectSettings()
		{
			AssetDatabase.Refresh();
			var d = System.IO.Directory.CreateDirectory(DataCollection.ProjectBuildPackagePath);
			string destination = EditorUtility.SaveFolderPanel("Choose a destination", d.FullName, "CreateNewFolderHere");//EditorUtility.SaveFilePanel("Choose a destination", d.FullName, "", "xml");
			if ( string.IsNullOrEmpty(destination) )
			{
				throw new Exception("无效的导出路径");
			}
			else
			{
				DataCollection.ExportProjectSettings(destination);
			}
			Debug.Log("Export Project Settings Succeed.");
		}


		[MenuItem("XDProject/ImportProjectSettings")]
		private static void ImportProjectSettings()
		{
			AssetDatabase.Refresh();

			
			string destination = EditorUtility.OpenFolderPanel("Choose a destination"
										, System.IO.Directory.CreateDirectory(DataCollection.ProjectBuildPackagePath).FullName
										, "");
			if ( string.IsNullOrEmpty(destination) )
			{
				throw new Exception("无效的文件路径");
			}
			else
			{
				DataCollection.ImportBuildSettingFromXML(System.IO.Path.Combine(destination, DataCollection.SettingFileName));
			}
			Debug.Log("Import ProjectSettings Succeed.");
		}
	}
}
