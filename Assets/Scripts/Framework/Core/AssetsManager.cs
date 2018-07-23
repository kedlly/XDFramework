using UnityEngine;
using Framework.Library.Singleton;
using Framework.Library.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Framework.Core
{
	[GameObjectPath("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class AssetsManager : ToSingletonBehavior<AssetsManager>
	{

		const string C_AssetBundleManifest = "AssetBundleManifest";

		public void Initalize()
		{
			
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.V))
			{
				//Debug.Log(ApplicationManager.Instance);
				LoadAsset();
			}
		}

		public void LoadAsset()
		{
			StartCoroutine(
			LoadFromManifest("Assets/StreamingAssets/StreamingAssets", "client.pfb", true)
			);
		}

		private void AddToBuffer(string name, UnityEngine.Object obj)
		{
			Instantiate(obj);
		}



		public void LoadAssetAsync<T>(string abPath, string assetName, Action<float, bool, T> OnLoadAsset) where T : UnityEngine.Object
		{
			StartCoroutine(LoadAsset<T>(abPath, assetName, OnLoadAsset, true));
		}

		public void LoadAssetSync<T>(string abPath, string assetName, Action<float, bool, T> OnLoadAsset) where T : UnityEngine.Object
		{
			StartCoroutine(LoadAsset<T>(abPath, assetName, OnLoadAsset, false));
		}


		//从 assetsbundle 中加载资源
		public IEnumerator LoadAsset<T>(string abPath, string assetName, Action<float, bool, T> OnLoadAsset, bool isAsync = true) where T : UnityEngine.Object
		{
			var bundle = AssetBundle.LoadFromFile(abPath);
			if (bundle != null)
			{
				if (isAsync)
				{
					AssetBundleRequest gObject = bundle.LoadAssetAsync<T>(assetName);

					if (OnLoadAsset != null)
					{
						OnLoadAsset(gObject.progress, gObject.isDone, gObject.asset as T);
					}

					yield return gObject;
// 					T obj = gObject.asset as T;
// 					if (obj == null)
// 					{
// 						Debug.LogErrorFormat("Cant load Asset with name {0} in AssetBundle : {1}, async = true", assetName, abPath);
// 						yield return null;
// 					}
// 					AddToBuffer(assetName, obj);
				}
				else
				{
					if (OnLoadAsset != null)
					{
						OnLoadAsset(0f, false, null);
					}
					T obj = bundle.LoadAsset<T>(assetName);
					if (OnLoadAsset != null)
					{
						OnLoadAsset(1f, true, obj);
					}
					// 					if (obj == null)
					// 					{
					// 						Debug.LogErrorFormat("Cant load Asset with name {0} in AssetBundle : {1}, async = false", assetName, abPath);
					// 						yield return null;
					// 					}
					// 					AddToBuffer(assetName, obj);
				}
			}
			bundle.Unload(false);
		}

		public void LoadAllAssetAsync<>

		//从 assetsbundle 中加载全部资源
		public IEnumerator loadAllAssets<T>(string abPath, bool isAsync = true) where T : UnityEngine.Object
		{
			var bundle = AssetBundle.LoadFromFile(abPath);
			T[] objs = null;
			if (isAsync)
			{
				AssetBundleRequest gObject = bundle.LoadAllAssetsAsync<T>();
				yield return gObject;
				objs = gObject.asset as T[];
				if (objs == null)
				{
					Debug.LogErrorFormat("Cant load Asset in AssetBundle : {0} async = true", abPath);
					yield return null;
				}
			}
			else
			{
				objs = bundle.LoadAllAssets<T>();
				if (objs == null)
				{
					Debug.LogErrorFormat("Cant load Asset AssetBundle : {0} async = false", abPath);
					yield return null;
				}
			}

			foreach (var obj in objs)
			{
				AddToBuffer(obj.name, obj);
			}

			bundle.Unload(false);
		}

		//从 assetsbundle 中加载相关依赖资源
		public IEnumerator LoadFromManifest(string manifestPath, string abName, bool load, bool isAsync = true)
		{
			//加载那个打包时额外打出来的总包
			// 			StartCoroutine(
			// 				loadAllAssets<GameObject>("Assets/StreamingAssets/client.pfb", false)
			// 			);
			AssetBundle bundle = null;
			if (isAsync)
			{
				var abcr = AssetBundle.LoadFromFileAsync(manifestPath);
				yield return abcr;
				bundle = abcr.assetBundle;
			}
			else
			{
				bundle = AssetBundle.LoadFromFile(manifestPath);
			}

			if (bundle == null)
			{
				Debug.LogErrorFormat("bundle cannot load : {0}", manifestPath);
				yield return null;
			}

			AssetBundleManifest manifest = null;
			if (isAsync)
			{
				var abr = bundle.LoadAssetAsync<AssetBundleManifest>(C_AssetBundleManifest);
				yield return abr;
				manifest = abr.asset as AssetBundleManifest;
			}
			else
			{
				manifest = bundle.LoadAsset<AssetBundleManifest>(C_AssetBundleManifest);
			}
			string[] k = manifest.GetAllAssetBundles();
			if (!k.Contains(abName))
			{
				Debug.LogErrorFormat("AssetBundle not found : {0}/{1}", manifestPath, abName);
				yield return null;
			}
			string[] deps = manifest.GetAllDependencies(abName);
			if (deps != null)
			{
				string path = manifestPath.Substring(0, manifestPath.LastIndexOf('/'));
				path += '/' + abName;
				foreach (var n in deps)
				{
					if (isAsync)
					{
						
					}
				}
			}
			bundle.Unload(true);
		}
			/*string[] deps = manifest.GetAllDependencies("client");
			//earth是打出的包名
			List<AssetBundle> depList = new List<AssetBundle>();
			Debug.Log(deps.Length);
			for (int i = 0; i < deps.Length; ++i)
			{
				AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundles/" + deps[i]);
				depList.Add(ab);
			}

			AssetBundle cubeAb = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundles/earth");
			GameObject org = cubeAb.LoadAsset("earth") as GameObject;
			Instantiate(org);

			cubeAb.Unload(false);
			for (int i = 0; i < depList.Count; ++i)
			{
				depList[i].Unload(false);
			}
			bundle.Unload(true);
		}
		public void LoadNoManifest()
		{
			var bundle = AssetBundle.LoadFromFile("Assets/AssetBundles/earth11");

			AssetBundleRequest gObject = bundle.LoadAssetAsync("earth11", typeof(GameObject));
			GameObject obj = gObject.asset as GameObject;
			Instantiate(obj);
			// Unload the AssetBundles compressed contents to conserve memory
			bundle.Unload(false);
		}
		/*public void loadWWWNoStored()
		{
			StartCoroutine(_loadWWWNoStored());
		}
		public void LoadWWWStored()
		{
			StartCoroutine(_LoadWWWStored());
		}
		/// <summary>
		/// -变量BundleURL的格式–如果为UnityEditor本地：
		/// —“file://”+“application.datapath”+“/文件夹名称/文件名”
		/// –如果为服务器： —http://….
		///-变量AssetName: 
		///–为prefab或文件的名字。
		/// </summary>
		public IEnumerator _loadWWWNoStored()
		{
			// Download the file from the URL. It will not be saved in the Cache
			using (WWW www = new WWW(BundleURL))
			{
				yield return www;
				if (www.error != null)
					throw new Exception("WWW download had an error:" + www.error);
				AssetBundle bundle = www.assetBundle;
				if (AssetName == "")
					Instantiate(bundle.mainAsset);
				else
					Instantiate(bundle.LoadAsset(AssetName));
				// Unload the AssetBundles compressed contents to conserve memory
				bundle.Unload(false);

			} // memory is freed from the web stream (www.Dispose() gets called implicitly)
		}
		public IEnumerator _LoadWWWStored()
		{
			// Wait for the Caching system to be ready
			while (!Caching.ready)
				yield return null;

			// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
			using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, //version/ 0))
			{
				yield return www;
				if (www.error != null)
					throw new Exception("WWW download had an error:" + www.error);
				AssetBundle bundle = www.assetBundle;
				if (AssetName == "")
					Instantiate(bundle.mainAsset);
				else
					//              Instantiate(bundle.mainAsset);

					//              Instantiate(bundle.LoadAsset(AssetName));
					foreach (UnityEngine.Object temp in bundle.LoadAllAssets())
					{
						Instantiate(temp);
					}
				//      Instantiate(bundle.LoadAllAssets());
				// Unload the AssetBundles compressed contents to conserve memory
				bundle.Unload(false);

			} // memory is freed from the web stream (www.Dispose() gets called implicitly)
		}*/
	}
}