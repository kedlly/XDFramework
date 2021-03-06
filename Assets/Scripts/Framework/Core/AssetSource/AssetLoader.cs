﻿
using System.Collections;
using System.IO;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;
using UnityEngine;

namespace Framework.Core
{
	[PathInHierarchyAttribute("/[Game]/AssetLoader"), DisallowMultipleComponent]
	public class AssetLoader2 : ToSingletonBehavior<AssetLoader2>
	{

		private string ConvertAssetPathTo3WPath(string path)
		{
#if UNITY_EDITOR

#endif
			return path;
		}

		private IEnumerator LoadAsset(string path)
		{
			WWW www = new WWW("file:///" + Application.streamingAssetsPath + Path.DirectorySeparatorChar + "QAssetBundle" + Path.DirectorySeparatorChar + "testab.unity3d");

			yield return www;

			if(string.IsNullOrEmpty(www.error))
			{
				var go = www.assetBundle.LoadAsset<GameObject>("Canvas");

				Instantiate(go);
			}
			else
			{
				Debug.LogError(www.error);
			}

		}

		
	}
}
