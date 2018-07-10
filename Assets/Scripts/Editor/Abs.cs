
using System.Collections.Generic;

using System.Text;

using System.IO;

namespace Framework.Editor
{
	//#if UNITY_5_MODE
	using UnityEngine;
	using UnityEditor;
	

	public class BuildAssetbundle
	{
		public static string m_destFileName = "Assetbundle";

#if UNITY_ANDROID
    public static string PlatformExtension = ".android";
    public static string Extension = ".x.android";
    public static BuildTarget Target = BuildTarget.Android;
    public static string ASSETBUNDLE_PATH = Application.dataPath + "/../AndroidResources/StreamingAssets";
    public static string FULL_ASSETBUNDLE_PATH = ASSETBUNDLE_PATH + "/" + m_destFileName;
#elif UNITY_IOS
    public static string PlatformExtension = ".ios";
    public static string Extension  = ".x.ios";
    public static BuildTarget Target = BuildTarget.iOS;
    public static string ASSETBUNDLE_PATH = Application.dataPath + "/../IosResources/StreamingAssets";
		public static string FULL_ASSETBUNDLE_PATH = ASSETBUNDLE_PATH + "/" + m_destFileName;
#else
		public static string PlatformExtension = ".win";
		public static string Extension = ".x.win";
		public static BuildTarget Target = BuildTarget.StandaloneWindows64;
		public static string ASSETBUNDLE_PATH = Application.dataPath + "/../win64/StreamingAssets";
		public static string FULL_ASSETBUNDLE_PATH = ASSETBUNDLE_PATH + "/" + m_destFileName;
#endif


#if BUILD_UI
    //需要打包的资源
    public static List<UIFont> fontList = new List<UIFont>();
    public static List<UIAtlas> atlasList = new List<UIAtlas>();
    public static List<GameObject> componentList = new List<GameObject>();
    public static List<GameObject> panelList = new List<GameObject>();
 
    public static void BuildOnePanel()
    {
        ClearAllBundleName();
        CleanTempData();
 
        GameObject[] selectGameObjects = Selection.gameObjects;
        UnityEngine.Object selectFloder = Selection.activeObject;
        if (selectGameObjects != null && selectGameObjects.Length != 0)
        {
            foreach (GameObject selGb in selectGameObjects)
            {
                string selGbPath = AssetDatabase.GetAssetPath(selGb);
                if (selGbPath.StartsWith(BuildUtils.PANEL_ASSET_PATH))
                {
                    if (selGbPath.EndsWith(BuildUtils.PREFAB_SUFFIX))
                    {
                        GameObject uiInstance = PrefabUtility.InstantiatePrefab(selGb) as GameObject;
                        CheckComponent(uiInstance);
                        FindRefrence(uiInstance.transform);
 
                        string prefabPath = BuildUtils.TEMP_PANEL_PREFAB_PATH + "/" + uiInstance.name + BuildUtils.PREFAB_SUFFIX;
                        PrefabUtility.CreatePrefab(prefabPath, uiInstance);
                        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                        panelList.Add(prefabAsset);
 
                        UnityEngine.Object.DestroyImmediate(uiInstance);
                    }
                    else
                    {
                        Debug.LogWarning("选择的资源 " + selGb.name + " 不是界面Prefab!");
                    }
                }
                else
                {
                    Debug.LogWarning("只能打包放在 " + BuildUtils.PANEL_ASSET_PATH + " 下面的界面Prefab");
                }
            }
            StartBuildGameObjects();
 
            BuildUtils.DeleteFileWithSuffixs(ASSETBUNDLE_PATH, new string[]{".manifest",""}, true, false);
            ShowBundleFolder();
            EditorUtility.DisplayDialog("提示", "打包完成！", "确定");
        }
        else
        {
            Debug.LogWarning("只能对 " + BuildUtils.PANEL_ASSET_PATH + " 下面的UI使用！");
        }
    }
 
    public static void BuildAllPanel(bool hint)
    {
        ClearAllBundleName();
 
        if (!hint || EditorUtility.DisplayDialog("提示", "确定打包 " + BuildUtils.PANEL_ASSET_PATH + " 下所有界面?", "确定", "取消"))
        {
            CleanTempData();
 
            string[] files = Directory.GetFiles(BuildUtils.PANEL_ASSET_PATH);
            if (files.Length != 0)
            {
                foreach (string file in files)
                {
                    if (file.EndsWith(BuildUtils.PREFAB_SUFFIX))
                    {
                        GameObject uiInstance = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file)) as GameObject;
                        string ui_panel_prefab_name = Path.GetFileNameWithoutExtension(file);
                        CheckComponent(uiInstance);
                        FindRefrence(uiInstance.transform);
 
                        string prefabPath = BuildUtils.TEMP_PANEL_PREFAB_PATH + "/" + uiInstance.name + BuildUtils.PREFAB_SUFFIX;
                        PrefabUtility.CreatePrefab(prefabPath, uiInstance);
                        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                        panelList.Add(prefabAsset);
 
                        UnityEngine.Object.DestroyImmediate(uiInstance);
                    }
                }
 
                StartBuildGameObjects();
                BuildUtils.DeleteFileWithSuffixs(ASSETBUNDLE_PATH, new string[]{".manifest",""}, true, false);
            }
            if (hint)
            {
                ShowBundleFolder();
                EditorUtility.DisplayDialog("提示", BuildUtils.PANEL_ASSET_PATH + " 下的界面界面全部打包完成！", "确定");
            }
        }
    }
 
    /// <summary>
    /// 对每一个要打包的Prefab保存组件信息，找出Atlas、Font等//
    /// </summary>
    /// <param name="com">COM.</param>
    static void FindRefrence(Transform com)
    {
        PrefabHierarchyInfo info = com.gameObject.AddComponent<PrefabHierarchyInfo>();
        info.transforms = com.gameObject.GetComponentsInChildren<Transform>(true);
        info.myuianchors = com.gameObject.GetComponentsInChildren<MyUIAnchor>(true);
        UILabel[] uilabels = com.gameObject.GetComponentsInChildren<UILabel>(true);
        UISprite[] uisprites = com.gameObject.GetComponentsInChildren<UISprite>(true);
        UITexture[] uitextures = com.gameObject.GetComponentsInChildren<UITexture>(true);
 
        foreach (UILabel lab in uilabels)
        {
            UIFont font = lab.bitmapFont;
            if (font == null)
            {
                Debug.LogWarning(com.gameObject.name + " 下出现未使用UIFont或者没有知道字体的UIlabel, " + lab.gameObject.name);
                continue;
            }
 
 
            string uifontPath = AssetDatabase.GetAssetPath(font);
            if (!uifontPath.StartsWith(BuildUtils.UIFONT_PREFAB_PATH))
                Debug.LogWarning("使用了没有放在" + BuildUtils.UIFONT_PREFAB_PATH + " 目录下的UIFont:" + uifontPath);
 
            if (!fontList.Contains(font))
            {
                fontList.Add(font);
            }
 
            lab.text = "";
        }
        foreach (UISprite sp in uisprites)
        {
            UIAtlas atlas = sp.atlas;
 
            if (atlas == null)
                continue;
 
            string atlasPath = AssetDatabase.GetAssetPath(atlas);
            if (!atlasPath.StartsWith(BuildUtils.ATLAS_PREFAB_PATH))
                Debug.LogWarning("使用了未放在" + BuildUtils.ATLAS_PREFAB_PATH + "目录下的Atlas:" + atlasPath);
 
            
            sp.RecordAtlasName = atlas.name;
 
            if (!atlasList.Contains(atlas))
                atlasList.Add(atlas);
        }
 
        foreach (UITexture t in uitextures)
        {
            t.mainTexture = null;
            t.shader = null;
        }
 
        com.parent = null;
    }
 
 
    /// <summary>
    /// 对找出的需要打包资源打包//
    /// </summary>
    private static void StartBuildGameObjects()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
 
        AssetbundleCommonFun.SetAssetBundlesName<UIFont>(fontList, Extension, false);
        AssetbundleCommonFun.SetAssetBundlesName<UIAtlas>(atlasList, Extension, false);
        AssetbundleCommonFun.SetAssetBundlesName<GameObject>(componentList, Extension, false);
        AssetbundleCommonFun.SetAssetBundlesName<GameObject>(panelList, Extension, false);
 
        AssetDatabase.Refresh();
        BuildAll();
 
        CleanTempData();
        ClearAllBundleName();
 
    }
 
 
    //在一次打包过程中，出现过的Component，检测多个Panel里面的（DynamicComponent）重名//
    static List<string> componentNames = new List<string>();
    //支持无限深度的Component嵌套
    static void CheckComponent(GameObject go)
    {
        string prefabPath = "";
        Dictionary<uint, List<Transform>> allComponent = new Dictionary<uint, List<Transform>>();
        FindComponents(go.transform, 0, ref allComponent);
        uint[] indexs = new uint[allComponent.Keys.Count];
        allComponent.Keys.CopyTo(indexs, 0);
        System.Array.Sort(indexs);
        for (int i = indexs.Length - 1; i >= 0; i--)
        {
            foreach (Transform com in allComponent[indexs[i]])
            {
                FindRefrence(com);
 
                prefabPath = BuildUtils.TEMP_COMPONENT_PREFAB_PATH + "/" + com.name + BuildUtils.PREFAB_SUFFIX;
                PrefabUtility.CreatePrefab(prefabPath, com.gameObject);
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                componentList.Add(prefabAsset);
            }
        }
 
        for (int i = indexs.Length - 1; i >= 0; i--)
        {
            foreach (Transform com in allComponent[indexs[i]])
            {
                UnityEngine.Object.DestroyImmediate(com.gameObject);
 
            }
        }
    }
 
    /// <summary>
    /// 递归寻找动态组件//
    /// </summary>
    /// <param name="tf">Tf.</param>
    /// <param name="index">Index.</param>
    /// <param name="dict">Dict.</param>
    static void FindComponents(Transform tf, uint index, ref Dictionary<uint, List<Transform>> dict)
    {
        if (tf.name.Contains("(DynamicComponent)"))
        {
            foreach (Transform sonTf in tf)
            {
                if (componentNames.Contains(sonTf.name))
                {
                    UnityEngine.Debug.LogError(tf.name + " 子物体中出现同名的动态组件 " + sonTf.name);
                    UnityEngine.Object.DestroyImmediate(sonTf);
                    continue;
                }
                if (!sonTf.name.Contains("(DynamicComponent)"))
                {
                    componentNames.Add(sonTf.name);
                    if (!dict.ContainsKey(index))
                        dict.Add(index, new List<Transform>());
                    dict[index].Add(sonTf);
                    if (sonTf.childCount > 0)
                        FindComponents(sonTf, index + 1, ref dict);
                }
                else
                {
                    if (sonTf.childCount > 0)
                        FindComponents(sonTf, index + 1, ref dict);
                }
            }
        }
        else {
            foreach (Transform sonTf in tf)
            {
                if (sonTf.childCount > 0)
                    FindComponents(sonTf, index + 1, ref dict);
            }
        }
    }
 
    static void CleanTempData()
    {
        fontList.Clear();
        atlasList.Clear();
        componentList.Clear();
        componentNames.Clear();
    }
#endif

		#region tools
		static void BuildAll()
		{
			EditorUtility.ClearProgressBar();
			var str = @Application.genuine;
			AssetbundleCommonFun.CreatePath(ASSETBUNDLE_PATH + "/Assetbundle");
			BuildPipeline.BuildAssetBundles(ASSETBUNDLE_PATH + "/Assetbundle", BuildAssetBundleOptions.ChunkBasedCompression, Target);

		}

		//[MenuItem("[AssetBundles]/Build Bundles Independent")]
		static void BuildBundlesIndependent()
		{
			List<Object> list = new List<Object>();
			list.AddRange(Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets));

			int count = list.Count;
			string temp_path;
			List<string> pathList = new List<string>();

			foreach(Object o in list)
			{
				if(o == null)
				{
					continue;
				}

				AssetbundleCommonFun.ChangeTextureFormat(o);
				temp_path = AssetDatabase.GetAssetPath(o);
				if(!pathList.Contains(temp_path))
				{
					pathList.Add(temp_path);
				}
			}

			BuildIndependent(pathList);
		}

		static void BuildIndependent(List<string> pathList)
		{
			string OutPutFolder = ASSETBUNDLE_PATH + "/Assetbundle";
			AssetbundleCommonFun.CreatePath(OutPutFolder);
			AssetBundleBuild temp_build;
			List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();

			int count = pathList.Count;
			int index = 0;

			foreach(string item in pathList)
			{
				if(item == null)
				{
					index++;
					continue;
				}

				EditorUtility.DisplayProgressBar("Build Bundles Independent", item, (float)index / (float)count);

				if(!string.IsNullOrEmpty(item))
				{
					temp_build = new AssetBundleBuild();
					temp_build.assetBundleName = AssetbundleCommonFun.GetBundleName(item, Extension);
					temp_build.assetNames = new string[] { item };
					abbs.Add(temp_build);
				}

				index++;
			}

			BuildPipeline.BuildAssetBundles(OutPutFolder, abbs.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, Target);

			string mainConfigPath = OutPutFolder + "/" + Path.GetFileName(OutPutFolder);
			if(File.Exists(mainConfigPath))
			{
				File.Delete(mainConfigPath);
			}

			AssetbundleCommonFun.ExportModifyFilesInfo(ASSETBUNDLE_PATH + "/Assetbundle");

			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("提示", "操作结束", "OK");
		}

		//[@MenuItem("[AssetBundles]/Set Name for self")]     此功能不开放
		static void SetNameForSelf()
		{
			List<Object> list = new List<Object>();
			list.AddRange(Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets));
			int count = list.Count;
			int index = 0;
			foreach(Object o in list)
			{
				if(o == null)
				{
					index++;
					continue;
				}

				string path = AssetDatabase.GetAssetPath(o);

				AssetbundleCommonFun.SetBundleName(path, Extension);

				index++;
				EditorUtility.DisplayProgressBar("Set Assetbundle Name", path, (float)index / (float)count);
			}

			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("提示", "操作结束", "OK");
		}

		//[@MenuItem("[AssetBundles]/Set Assetbundle Name")]     此功能不开放
		static void SetName()
		{
			List<Object> list = new List<Object>();
			list.AddRange(Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets));
			int count = list.Count;
			int index = 0;
			foreach(Object o in list)
			{
				if(o == null)
				{
					index++;
					continue;
				}

				string path = AssetDatabase.GetAssetPath(o);

				AssetbundleCommonFun.SetBundleName(path, Extension);

				index++;
				EditorUtility.DisplayProgressBar("Set Assetbundle Name", path, (float)index / (float)count);
			}

			EditorUtility.ClearProgressBar();
			//EditorUtility.DisplayDialog("提示", "操作结束", "OK");
		}


		//[@MenuItem("[AssetBundles]/Clear Selected AssetbundleName")]
		static void ClearName()
		{
			List<Object> list = new List<Object>();
			list.AddRange(Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets));
			int count = list.Count;
			int index = 0;
			foreach(Object o in list)
			{
				if(o == null)
				{
					index++;
					continue;
				}

				string path = AssetDatabase.GetAssetPath(o);

				AssetbundleCommonFun.SetBundleName(path, "", true);

				//AssetbundleCommonFun.SetDependenciesName(path, "", false, true);

				index++;
				EditorUtility.DisplayProgressBar("Clear Assetbundle Name", path, (float)index / (float)count);
			}

			EditorUtility.ClearProgressBar();
			//EditorUtility.DisplayDialog("提示", "操作结束", "OK");
		}

		//[@MenuItem("[AssetBundles]/Clear All AssetbundleNames")]
		static void ClearAllBundleName()
		{
			string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
			List<string> hasBundleNameAssets = new List<string>();
			foreach(string n in allBundleNames)
			{
				foreach(string p in AssetDatabase.GetAssetPathsFromAssetBundle(n))
				{
					hasBundleNameAssets.Add(p);
				}
			}
			float idx = 0f;
			foreach(string asset in hasBundleNameAssets)
			{
				AssetbundleCommonFun.SetBundleName(asset, "", true);
				EditorUtility.DisplayProgressBar("清除所有Bundle名称", "当前处理文件:" + Path.GetFileName(asset), idx++ / hasBundleNameAssets.Count);
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.RemoveUnusedAssetBundleNames();
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			//EditorUtility.DisplayDialog("提示", "操作结束", "OK");
		}


		/// <summary>
		/// 显示Bundle文件夹//
		/// </summary>
		static void ShowBundleFolder()
		{
			string path = FULL_ASSETBUNDLE_PATH + "/assets";
			path = path.Replace("/", "\\");
			Debug.Log(path);
			global::System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
		}
		#endregion
	}
	//#endif

	static class TESTCls
	{
		[MenuItem("Tool/Test")]
		static void test()
		{
			string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
			List<string> hasBundleNameAssets = new List<string>();
			foreach(string n in allBundleNames)
			{
				foreach(string p in AssetDatabase.GetAssetPathsFromAssetBundle(n))
				{
					hasBundleNameAssets.Add(p);
				}
			}
			float idx = 0f;
			foreach(string asset in hasBundleNameAssets)
			{
				//SetBundleName(asset, "", false);
				EditorUtility.DisplayProgressBar("清除所有Bundle名称", "当前处理文件:" + Path.GetFileName(asset), idx++ / hasBundleNameAssets.Count);
			}
			EditorUtility.ClearProgressBar();
		}
	}

	public class AssetbundleCommonFun
	{
#if UNITY_ANDROID
    private static string strLocalFileName = "Android_LocalFilesInfo.asset";
#elif UNITY_IOS
    private static string strLocalFileName = "Ios_LocalFilesInfo.asset";
#endif

		private static List<string> componentList = new List<string>();
		static AssetImporter m_importer = null;


		public static void ClearAllBundleName()
		{
			string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
			List<string> hasBundleNameAssets = new List<string>();
			foreach(string n in allBundleNames)
			{
				foreach(string p in AssetDatabase.GetAssetPathsFromAssetBundle(n))
				{
					hasBundleNameAssets.Add(p);
				}
			}
			float idx = 0f;
			foreach(string asset in hasBundleNameAssets)
			{
				SetBundleName(asset, "", false);
				EditorUtility.DisplayProgressBar("清除所有Bundle名称", "当前处理文件:" + Path.GetFileName(asset), idx++ / hasBundleNameAssets.Count);
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.RemoveUnusedAssetBundleNames();
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}

		public static void CreatePath(string path)
		{
			string NewPath = path.Replace("\\", "/");

			string[] strs = NewPath.Split('/');
			string p = "";

			for(int i = 0; i < strs.Length; ++i)
			{
				p += strs[i];

				if(i != strs.Length - 1)
				{
					p += "/";
				}

				if(!Path.HasExtension(p))
				{
					if(!Directory.Exists(p))
						Directory.CreateDirectory(p);
				}

			}
		}

		public static string SetBundleName(string path, string name, bool isForce = false)
		{
			if(!isForce)
			{
				if(Directory.Exists(path))
				{
					return null;
				}
			}

			string dictName = Path.GetDirectoryName(path);
			string fileName = Path.GetFileNameWithoutExtension(path);
			string extension = Path.GetExtension(path);

			dictName = dictName.Replace("UIResources_temp", "UIResources");

			if(!isForce)
			{
				if(extension.Equals(".dll") || extension.Equals(".cs") || extension.Equals(".js") || (name != "" && fileName.Contains("atlas") && !extension.Equals(".prefab")))
				{
					return null;
				}
			}

			m_importer = AssetImporter.GetAtPath(path);

			if(name != "")
			{
				m_importer.assetBundleName = dictName + "/" + fileName + name;
			}
			else
			{
				m_importer.assetBundleName = "";
			}
			AssetDatabase.Refresh();

			return m_importer.assetBundleName;
		}

		public static string GetBundleName(string path, string name, bool isForce = false)
		{
			string retBundleName = null;

			if(!isForce)
			{
				if(Directory.Exists(path))
				{
					return retBundleName;
				}
			}

			string dictName = Path.GetDirectoryName(path);
			string fileName = Path.GetFileNameWithoutExtension(path);
			string extension = Path.GetExtension(path);

			if(!isForce)
			{
				if(extension.Equals(".dll") || extension.Equals(".cs") || extension.Equals(".js") || (name != "" && fileName.Contains("atlas") && !extension.Equals(".prefab")))
				{
					return null;
				}
			}

			if(name != "")
			{
				retBundleName = dictName + "/" + fileName + name;

				//Object tex = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

				//if (tex is Texture2D)
				//{
				//    SetTexture(tex as Texture2D);
				//}
			}

			Debug.Log("Asset name: " + fileName);

			AssetDatabase.Refresh();

			return retBundleName;
		}


		static void FindComponents(Transform tf, uint index, ref Dictionary<uint, List<Transform>> dict)
		{
			if(tf.name.Contains("(DynamicComponent)"))
			{
				foreach(Transform sonTf in tf)
				{
					if(componentList.Contains(sonTf.name))
					{
						Debug.LogWarning("same name component...");
						UnityEngine.Object.DestroyImmediate(sonTf);
						continue;
					}
					if(!sonTf.name.Contains("(DynamicComponent)"))
					{
						Debug.Log("找到组件" + sonTf.name + "深度" + index);
						componentList.Add(sonTf.name);
						if(!dict.ContainsKey(index))
							dict.Add(index, new List<Transform>());
						dict[index].Add(sonTf);
						if(sonTf.childCount > 0)
							FindComponents(sonTf, index + 1, ref dict);
					}
					else
					{
						if(sonTf.childCount > 0)
							FindComponents(sonTf, index + 1, ref dict);
					}
				}
			}
			else
			{
				foreach(Transform sonTf in tf)
				{
					if(sonTf.childCount > 0)
						FindComponents(sonTf, index + 1, ref dict);
				}
			}
		}


		public static void SetTexture(Texture2D tex)
		{
			string path = AssetDatabase.GetAssetPath(tex);
			TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

			if(importer != null)
			{
				if(!(importer.textureType == TextureImporterType.NormalMap || importer.normalmap))
					importer.mipmapEnabled = false;
				importer.npotScale = TextureImporterNPOTScale.ToNearest;
				importer.textureType = TextureImporterType.Default;
				importer.spriteImportMode = SpriteImportMode.None;
				importer.isReadable = false;

				if(path.Contains("_alpha8"))
				{
					importer.textureFormat = TextureImporterFormat.Alpha8;
				}
				else
				{
					importer.textureFormat = SetTextureFormat(tex, importer);
				}
				importer.maxTextureSize = 8192;

				AssetDatabase.ImportAsset(path);
			}
		}


		static TextureImporterFormat SetTextureFormat(Texture t, TextureImporter importer)
		{
			TextureImporterFormat TextureFormat;
#if UNITY_IPHONE
        if (fun(t.width) && fun(t.height))
        {
            if (importer.DoesSourceTextureHaveAlpha())
            {
                TextureFormat = TextureImporterFormat.PVRTC_RGBA4;
            }
            else
            {
                TextureFormat = TextureImporterFormat.PVRTC_RGB4;
            }
        }
        else
        {
            TextureFormat = TextureImporterFormat.ETC2_RGBA8;
                    
        }
                    
#elif UNITY_ANDROID
        if (fun(t.width) && fun(t.height))
        {
            if (importer.DoesSourceTextureHaveAlpha())
            {
                //importer.alphaIsTransparency = true;
                TextureFormat = TextureImporterFormat.ETC2_RGBA8;
            }
            else
                TextureFormat = TextureImporterFormat.ETC_RGB4;
        }
        else {
            if (importer.DoesSourceTextureHaveAlpha())
            {
                //importer.alphaIsTransparency = true;
                TextureFormat = TextureImporterFormat.RGBA16;
            }
            else
                TextureFormat = TextureImporterFormat.RGB16;
            Debug.LogWarning("Texture " + t.name + " 尺寸不为2的幂次方，无法使用ETC压缩,当前使用 " + TextureFormat.ToString());
        }
#else
			TextureFormat = TextureImporterFormat.RGBA32;
#endif
			return TextureFormat;
		}


		static bool fun(int v)
		{
			bool flag = false;
			if((v > 0) && (v & (v - 1)) == 0)
				flag = true;
			return flag;
		}

		public static void ExportModifyFilesInfo(string path)
		{
			string extension = "";
			string[] retFils = Directory.GetFiles(path, "*.manifest", SearchOption.AllDirectories);
			//LocalFilesInfo filesInfo = ScriptableObject.CreateInstance<LocalFilesInfo>();

			foreach(var item in retFils)
			{
				File.Delete(item);
			}
		}

		private static void FileCopy(string sourceFileName, string destFileName)
		{
			string dictName = Path.GetDirectoryName(destFileName);

			CreatePath(dictName);

			File.Copy(sourceFileName, destFileName);
		}

		private static string GetMD5HashFromFile(string fileName)
		{
			try
			{
				FileStream file = new FileStream(fileName, FileMode.Open);
				global::System.Security.Cryptography.MD5 md5 = new global::System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();

				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch(global::System.Exception ex)
			{
				throw new global::System.Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
			}
		}

		public static void ChangeTextureFormat(Object obj)
		{
			Object[] dependObjects;
			dependObjects = EditorUtility.CollectDependencies(new Object[] { obj });

			foreach(Object val in dependObjects)
			{
				if(val is Texture2D)
				{
					SetTexture(val as Texture2D);
				}
			}

			AssetDatabase.Refresh();
		}

		public static void SetAssetBundlesName<T>(List<T> list, string Extension, bool needRefresh) where T : Object
		{
			for(int i = 0; i < list.Count; i++)
			{
				SetAssetBundleName(list[i], Extension, needRefresh);
			}
		}

		public static void SetAssetBundleName<T>(T asset, string Extension, bool needRefresh) where T : Object
		{
			string path = AssetDatabase.GetAssetPath(asset);
			SetBundleName(path, Extension, needRefresh);
		}
	}


}
