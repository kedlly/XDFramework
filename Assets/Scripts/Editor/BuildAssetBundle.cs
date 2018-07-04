using UnityEditor;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Framework.Editor
{

	public static class AssetBundleBuildEditor
	{
		[MenuItem("Framework/AssetBundle/Build")]
		public static void BuildAssetBundle()
		{
			// AB包输出路径
			string outPath = Application.streamingAssetsPath + "/AssetBundles";

			// 检查路径是否存在
			CheckDirAndCreate(outPath);

			BuildPipeline.BuildAssetBundles(outPath, 0, EditorUserBuildSettings.activeBuildTarget);

			/*
			//获取在Project视图中选择的所有游戏对象  
			Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

			//遍历所有的游戏对象  
			foreach(Object obj in SelectedAsset)
			{
				string sourcePath = AssetDatabase.GetAssetPath(obj);
				//本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径  
				//StreamingAssets是只读路径，不能写入  
				//服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。  
				string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";
				uint crc = 0;
				if(BuildPipeline.BuildAssetBundle(obj, null, targetPath, out crc, BuildAssetBundleOptions.CollectDependencies , EditorUserBuildSettings.activeBuildTarget))
				{
					Debug.Log(obj.name + "资源打包成功");
				}
				else
				{
					Debug.Log(obj.name + "资源打包失败");
				}
			}*/
			//刷新编辑器  
			AssetDatabase.Refresh();

		}

		/// <summary>
		/// 判断路径是否存在,不存在则创建
		/// </summary>
		public static void CheckDirAndCreate(string dirPath)
		{
			if(!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
		}
	}
}