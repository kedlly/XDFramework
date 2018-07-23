
using System.Collections.Generic;

using System.Text;

using System.IO;
//#if UNITY_5_MODE
using UnityEngine;
using UnityEditor;
namespace Framework.Editor
{
	


	public class _BuildAssetbundle
	{
		[MenuItem("Tool/buildAssetbundle")]
		static void test()
		{
			BuildPipeline.BuildAssetBundles("./Assets/StreamingAssets"
				, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ChunkBasedCompression
				, BuildTarget.StandaloneWindows64);
		}
	}

	

	

}
