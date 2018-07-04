using Framework.Utils.Extensions;
using Projects.DataStruct.Courseware;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Projects.Course
{
	public class CourseProxy
	{

		Dictionary<string, GameObject> modeImage = new Dictionary<string, GameObject>();

		public GameObject Model { get; private set; }
		public Courseware CoursewareData { get; private set; }

		private void LoadModel(GameObject model)
		{
			Model = model;
			var meshes = model.GetComponentsInChildren<MeshRenderer>();
			for(int i = 0; i < meshes.Length; ++i)
			{
				modeImage.Add(meshes[i].gameObject.GetPathToParent(model), meshes[i].gameObject);
			}
		}

		protected CourseProxy()
		{
			IsReady = true;
		}

		public static CourseProxy Open(string name)
		{
			if (CourseLoader.GetAllCoursewares().Contains(name))
			{
				var cd = CourseLoader.OpenCourseware(name);
				if(cd != null)
				{
					var proxy = new CourseProxy();
					proxy.CoursewareData = cd;
					proxy.LoadCourseware();
					return proxy;
				}
			}
			return null;
		}

		public static CourseProxy New(string name)
		{
			if(!CourseLoader.GetAllCoursewares().Contains(name))
			{				
				var cd  = CourseLoader.NewCourseware(name);
				if(cd.Title.IsNotNullAndEmpty())
				{
					var proxy = new CourseProxy();
					proxy.CoursewareData = cd;
					return proxy;
				}
			}
			return null;
		}

		public void Save()
		{
			CourseLoader.SaveCourseware(CoursewareData);
		}

		public bool IsEditorMode { get { return CourseLoader.IsEditorMode; } }
		public bool IsReady { get; private set; }


		void LoadCourseware()
		{
			IsReady = false;
			var bundlePath = Path.Combine(CourseLoader.ResourceDirectory, CoursewareData.ResourceFilePath);
			if(!Directory.Exists(bundlePath))
			{
				Debug.LogError("{0} not exist".FormatEx(bundlePath));
				IsReady = true;
				return;
			}
			var bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);

			bundleRequest.completed += it =>
			{
				if(it.isDone)
				{
					var assetBundle = (it as AssetBundleCreateRequest).assetBundle;

					var devicePerfab = assetBundle.LoadAsset<GameObject>(CoursewareData.ResourceName);
					var currentDevice = GameObject.Instantiate(devicePerfab);
					LoadModel(currentDevice);
					IsReady = true;
				}
			};
		}

		GameObject[] GetObjectsInKnowledge(Knowledge knowledge)
		{
			return knowledge.itemPartMeshPathnameList == null || knowledge.itemPartMeshPathnameList.Count == 0 || ! IsReady ? new GameObject[0] :
				knowledge.itemPartMeshPathnameList.Select(
							it =>
							{
								GameObject outValue = null;
								modeImage.TryGetValue(it, out outValue);
								return outValue;
							}
						).ToArray();
		}

		public Knowledge AddKnowledge(string name, GameObject[] objs)
		{
			var newKnowledge = new Knowledge()
			{
				name = name
				, itemPartMeshPathnameList = objs != null ? objs.Select(o => o.GetPathToParent(Model)).ToList() : new List<string>()
			};
			var succeed = CoursewareData.AddKnowledge(newKnowledge);
			var log = "Add New Knownledge named : {0} to Courseware with Title : {1} {2}".FormatEx(name, CoursewareData.Title, succeed ? "succeed" : "failed");
			Debug.Log(log);
			return newKnowledge;
		}
	}
}
