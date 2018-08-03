using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils.Extensions
{
	public static partial class ExtensionUtils
	{
		/// <summary>
		/// 将当前Camera渲染的图像保存到2D纹理
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="rect">制定纹理大小</param>
		/// <returns></returns>
		public static Texture2D CaptureCamera(this Camera camera, Rect rect)
		{
			// 创建一个RenderTexture对象
			var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
			// 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
			camera.targetTexture = renderTexture;
			camera.Render();

			// 激活这个RenderTexture, 并从中中读取像素。  
			RenderTexture.active = renderTexture;

			// 先创建一个的空纹理，大小可根据实现需要来设置
			var screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
			screenShot.ReadPixels(rect, 0, 0);
			screenShot.Apply();

			// 重置相关参数，以使用camera继续在屏幕上显示  
			camera.targetTexture = null;
			RenderTexture.active = null;
			UnityEngine.Object.Destroy(renderTexture);

			return screenShot;
		}

		public static Texture2D CaptureCameras(this Camera[] cameras, Rect rect, bool useDepth = false, bool useStencil = false)
		{
			// 临时RenderTexture对象
			int rtDepth = useStencil ? 24 : (useDepth ? 16 : 0);
			var renderTexture = new RenderTexture(Screen.width, Screen.height, rtDepth);
			cameras.ForEach(
				delegate (Camera camera)
				{
					// 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机 
					camera.targetTexture = renderTexture;
					camera.Render();
					// 重置相关参数，以使用camera继续在屏幕上显示  
					camera.targetTexture = null;
				}
			);


			// 激活这个RenderTexture, 并从中中读取像素。  
			RenderTexture.active = renderTexture;

			// 先创建一个的空纹理，大小可根据实现需要来设置
			var screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
			screenShot.ReadPixels(rect, 0, 0);
			screenShot.Apply();

			RenderTexture.active = null;
			//释放临时RenderTextures
			UnityEngine.Object.Destroy(renderTexture);

			return screenShot;
		}

		/// <summary>
		/// HTML 字符串转 Color 值  
		/// example :  "#C5563CFF".HtmlStringToColor();
		/// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
		/// </summary>
		/// <param name="htmlString"></param>
		/// <returns></returns>
		public static Color HtmlStringToColor(this string htmlString)
		{
			Color retColor;
			var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out retColor);
			return parseSucceed ? retColor : Color.black;
		}

#region repair

		public static Dictionary<string, T> GetComponentCollection<T>(this Transform trans, string parentName = "", char pathSeparateChar = '/') where T : Component
		{
			Dictionary<string, T> ret = new Dictionary<string, T>();
			var currentTransName = parentName + pathSeparateChar + trans.name;
			var selfComponent = trans.GetComponent<T>();
			if(selfComponent != null)
			{
				ret.Add(currentTransName, selfComponent);
			}
			for(int i = 0; i < trans.childCount; ++i)
			{
				var child = trans.GetChild(i);
				var subItems = GetComponentCollection<T>(child, currentTransName);
				foreach(var item in subItems)
				{
					ret.Add(item.Key, item.Value);
				}
			}
			return ret;
		}

		public static Dictionary<string, T> GetComponentCollection<T>(this GameObject go, string parentName = "", char pathSeparateChar = '/') where T : Component
		{
			return GetComponentCollection<T>(go.transform, parentName, pathSeparateChar);
		}

		public static Dictionary<string, T> GetComponentCollectionFullPath<T>(this Transform rootTrans, char pathSeparateChar = '/') where T : Component
		{
			return GetComponentCollection<T>(rootTrans, rootTrans.parent.GetPath(pathSeparateChar), pathSeparateChar);
		}

		public static Dictionary<string, T> GetComponentCollectionFullPath<T>(this GameObject rootGo, char pathSeparateChar = '/') where T : Component
		{
			return GetComponentCollectionFullPath<T>(rootGo.transform, pathSeparateChar);
		}
#endregion

		public static string GetPath(this Transform selfTrans, char pathSeparateChar = '/')
		{
			ObjectPathOperatorWrapper wrapper = new ObjectPathOperatorWrapper(selfTrans.gameObject, pathSeparateChar);
			return wrapper.GetPathToParent((Transform)null);
		}

		public static string GetPath(this GameObject go, char pathSeparateChar = '/')
		{
			ObjectPathOperatorWrapper wrapper = new ObjectPathOperatorWrapper(go, pathSeparateChar);
			return wrapper.GetPathToParent((GameObject)null);
		}

		public static string GetPathToParent(this GameObject go, GameObject parent, char pathSeparateChar = '/')
		{
			ObjectPathOperatorWrapper wrapper = new ObjectPathOperatorWrapper(go, pathSeparateChar);
			return wrapper.GetPathToParent(parent);
		}


		public static GameObject FindGameObject(this string path, GameObject root, bool createIfNotExist = false)
		{
			return root.GetSubObject(path, createIfNotExist);
		}

		private class ObjectPathOperatorWrapper
		{
			const char DefaultPathSeparateChar = '/';
			
			public GameObject Current { get; private set; }
			public char PathSeparateChar { get; private set; }

			public ObjectPathOperatorWrapper(GameObject current, char pathSeparateChar)
			{
				Current = current;
				PathSeparateChar = pathSeparateChar;
			}

			public ObjectPathOperatorWrapper(GameObject current) : this(current, DefaultPathSeparateChar) { }


			private string[] GetDirectory(string path)
			{
				return Array.FindAll(path.Split(PathSeparateChar), s => s.IsNotNullAndEmpty());
			}

			private static GameObject __FindSubObject(GameObject root, string subObjectName, bool createIfNotExist = false)
			{
				var targetTrans = __FindSubObject(root != null ? root.transform : null, subObjectName, createIfNotExist);
				return targetTrans != null ? targetTrans.gameObject : null;
			}

			private static Transform __FindSubObject(Transform root, string subObjectName, bool createIfNotExist = false)
			{
				
				if (subObjectName.IsNullOrEmpty())
				{
					Debug.LogWarningFormat("Find child GameObject with no name at {0}, fix it and again.", root.name);
				}

				Transform ret = null;

				if (root != null)
				{
					for(int i = 0; i < root.childCount; ++i)
					{
						var indexObject = root.GetChild(i);
						if(indexObject.name == subObjectName)
						{
							ret = indexObject;
							break;
						}
					}
					if (ret == null && createIfNotExist)
					{
						ret = new GameObject(subObjectName).transform;
						ret.parent = root;
					}
				}
				else
				{
					var existObj = GameObject.Find('/' + subObjectName);
					if(existObj != null)
					{
						ret = existObj.transform;
					}
					if (ret == null && createIfNotExist)
					{
						ret = new GameObject(subObjectName).transform;
					}
				}
				return ret;
			}

			public GameObject Walk(string path, bool createIfNotExist = false)
			{
				GameObject ret = Current;
				if (path.IsNotNullAndEmpty())
				{
					var directory = GetDirectory(path);
					var tmp = Current;
					for (int i = 0; i < directory.Length; ++ i)
					{
						tmp = __FindSubObject(tmp, directory[i], createIfNotExist);
						if (tmp == null)
						{
							break;
						}
					}
					ret = tmp;
				}
				return ret;
			}


			public string GetPathToParent(Transform parentTrans)
			{
				return Current == null ? "" : __GetPathToParent(Current.transform, parentTrans, PathSeparateChar);
			}

			public string GetPathToParent(GameObject parent)
			{
				return GetPathToParent(parent != null ? parent.transform : null);
			}

			public static string __GetPathToParent(Transform selfTrans, Transform parent, char pathSeparateChar)
			{
				Transform trans = selfTrans;
				var path = "";
				while(trans != null && trans != parent)
				{
					if(trans.name.IsNullOrEmpty())
					{
						throw new Exception("Find child GameObject with no name at {0}, fix it and again.".FormatEx(selfTrans.name));
					}
					path = path.AddPrefix(pathSeparateChar + trans.name);
					trans = trans.parent;
				}
				return path;
			}

		}


		public static List<GameObject> GetAllObjectsInScene(bool bOnlyRoot)
		{
			GameObject[] pAllObjects = Resources.FindObjectsOfTypeAll<GameObject>();

			List<GameObject> pReturn = new List<GameObject>();

			foreach(GameObject pObject in pAllObjects)
			{
				if(bOnlyRoot)
				{
					if(pObject.transform.parent != null)
					{
						continue;
					}
				}

				if(pObject.hideFlags == HideFlags.NotEditable || pObject.hideFlags == HideFlags.HideAndDontSave)
				{
					continue;
				}
#if UNITY_EDITOR
				if(Application.isEditor)
				{
					string sAssetPath = UnityEditor.AssetDatabase.GetAssetPath(pObject.transform.root.gameObject);
					if(!string.IsNullOrEmpty(sAssetPath))
					{
						continue;
					}
				}
#endif

				pReturn.Add(pObject);
			}

			return pReturn;
		}

		

		public static T AddSubObjectComponent<T>(this GameObject root, string path, char pathSeparateChar = '/') where T : Component
		{
			var obj = root.AddSubObject(path, pathSeparateChar);
			return obj.AddComponent<T>();
		}

		public static GameObject AddSubObject(this GameObject parent, string path, char pathSeparateChar = '/')
		{
			ObjectPathOperatorWrapper wrapper = new ObjectPathOperatorWrapper(parent, pathSeparateChar);
			return wrapper.Walk(path, true);
		}

		public static GameObject GetSubObject(this GameObject root, string path, bool createIfNotExist = false, char pathSeparateChar = '/')
		{
			return  FindSubObject(root, path, createIfNotExist, pathSeparateChar) ;
		}

		public static GameObject FindSubObject(GameObject parent, string path, bool createIfNotExist = false, char pathSeparateChar = '/')
		{
			ObjectPathOperatorWrapper wrapper = new ObjectPathOperatorWrapper(parent, pathSeparateChar);
			return wrapper.Walk(path, createIfNotExist);
		}
	}

	public static class AutoFactory
	{
		public static T Create<T>() where T : Component
		{
			var info = typeof(T);
			T instance = null;
			var attributes = info.GetCustomAttributes(typeof(DontDestroyOnLoadAttribute), true);
			var dontDestroyOnLoad = (attributes != null && attributes.Length > 0);
			attributes = info.GetCustomAttributes(typeof(PathInHierarchyAttribute), true);
			GameObject go = null;
			foreach (var atribute in attributes)
			{
				var defineAttri = atribute as PathInHierarchyAttribute;
				if (defineAttri == null)
				{
					continue;
				}
				go = defineAttri.Path.Trim().FindGameObject(null, true);
				break;
			}
			if (go == null)
			{
				go = new GameObject(info.Name);
			}
			if (dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(go.transform.root.gameObject);
			}
			instance = go.AddComponent<T>();
			return instance;
		}
	}
}