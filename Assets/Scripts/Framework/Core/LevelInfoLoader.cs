
using UnityEngine;
using Framework.Library.Singleton;
using UnityEngine.SceneManagement;
using Framework.Utils.Extensions;
using System;
using System.Collections;
using Framework.Library.ObjectPool;
using System.Collections.Generic;
using Framework.Core.FlowControl;

namespace Framework.Client
{
	[Serializable]
	public class LevelInfo
	{
		public string mainSceneName;
		public string[] subScenes;
		public string LevelScript;
		public string LevelStateMachine;
	}

	public interface IAsyncOperation
	{
		float progress { get; }
		void OnLoadingDone();
		void LoadAsync();
	}

	class testLoader : IAsyncOperation
	{

		AsyncOperation op;

		float IAsyncOperation.progress
		{
			get
			{
				return op.progress  < 0.8 ? op.progress : 1.0f;
			}
		}

		void IAsyncOperation.LoadAsync()
		{
			op = SceneManager.LoadSceneAsync("SYART/Scene/Station2", LoadSceneMode.Single);
		}

		void IAsyncOperation.OnLoadingDone()
		{
			Debug.Log("Extern loading done.");
		}

	}

	public class SceneLoadingInfo : IPoolable
	{
		const float ProgressThreshold = 0.9f;
		internal AsyncOperation asyncOperation;
		internal IAsyncOperation externalAsyncOperations = null;

		public Action<int> progressActionByPercent;

		public string sceneName;
		public LoadSceneMode mode = LoadSceneMode.Single;

		internal event Action<SceneLoadingInfo> OnActivated;
		public event Action<SceneLoadingInfo> OnSceneLoadingDone;

		public bool isLoadingAsyncDone = false;
		private bool isActived = false;
		public void Activate()
		{
			if (!isLoadingAsyncDone || isActived)
			{
				return;
			}
			if (asyncOperation != null)
			{
				asyncOperation.allowSceneActivation = true;
			}
			if (OnActivated != null)
			{
				OnActivated(this);
			}
			isActived = true;
		}

		public IEnumerator LoadAsync(string sceneName
					, LoadSceneMode mode = LoadSceneMode.Single
					, IAsyncOperation externalAsyncOperations = null
					, Action<int> progressAction = null
				)
		{
			this.sceneName = sceneName;
			this.mode = mode;
			progressActionByPercent = progressAction;
			this.externalAsyncOperations = externalAsyncOperations;
			return LoadAsync();
		}

		public IEnumerator LoadAsync()
		{
			if (sceneName.IsNullOrEmpty())
			{
				throw new Exception("Scene Name cannot be null or empty");
			}
			yield return waitForEndOfFrame;
			asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
			asyncOperation.allowSceneActivation = false;
			yield return waitForEndOfFrame;
			yield return LoadingAsync();
		}

		public static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();


		private IEnumerator ProgressRuning(int targetPercent, int currentPercent)
		{
			while (targetPercent < currentPercent)
			{
				++targetPercent;
				if (progressActionByPercent != null)
				{
					progressActionByPercent(targetPercent);
				}
				yield return waitForEndOfFrame;
			}
		}

		private IEnumerator LoadingAsync()
		{
			isLoadingAsyncDone = false;
			int currentPercent = 0;
			int targetPercent = 0;
			//加载场景资源
			while (asyncOperation.progress < ProgressThreshold) //是否加载完成
			{
				//若未加载完成，则回调进度Action
				currentPercent = (int)(asyncOperation.progress * 100);
				yield return ProgressRuning(targetPercent, currentPercent);
				targetPercent = currentPercent;
			}

			//资源加载完毕，尝试加载场景扩展数据

			var startPoint = targetPercent;
			var endPoint = 100;
			var distance = endPoint - targetPercent;

			if (externalAsyncOperations != null)
			{
				externalAsyncOperations.LoadAsync();
				{
					while (externalAsyncOperations.progress < 1f) //是否加载完成
					{
						//若未加载完成，则回调进度Action
						currentPercent = (int)(externalAsyncOperations.progress * distance) + startPoint;
						yield return ProgressRuning(targetPercent, currentPercent);
						targetPercent = currentPercent;
					}
				}
			}
			
			yield return ProgressRuning(targetPercent, endPoint);
			targetPercent = endPoint;
			
			
			isLoadingAsyncDone = true;
			if (OnSceneLoadingDone != null)
			{
				OnSceneLoadingDone(this);
			}
			if (mode == LoadSceneMode.Single)
			{
				Activate();
			}

		}

		void IPoolable.OnAllocated()
		{
			Clean();
		}

		void IPoolable.OnRecycled()
		{
			Clean();
		}

		private void Clean()
		{
			progressActionByPercent = null;
			OnActivated = null;
			asyncOperation = null;
			mode = LoadSceneMode.Single;
			sceneName = null;
			isActived = isLoadingAsyncDone = false;
		}
	}

	[PathInHierarchy("/[Game]/LevelManager"), DisallowMultipleComponent]
	public class LevelManager : ToSingletonBehavior<LevelManager>
	{
		protected override void OnSingletonInit()
		{
// 			OnSceneLoaded += (s, m) =>
// 			{
// 				Debug.Log("OB_SceneLoaded  " + s.name + " / " + m);
// 			};
// 			OnSceneUnloaded += s =>
// 			{
// 				Debug.Log("OB_SceneUnLoaded  " + s.name);
// 			};
// 			OnSceneChanaged += (s1, s2) =>
// 			{
// 				Debug.Log("OB_SceneChanged  " + s1.name + " / " + s2.name);
// 			};
		}

		readonly Dictionary<string, SceneLoadingInfo> Dictionary = ObjectCache.GlobalCache.Allocate<Dictionary<string, SceneLoadingInfo>>();

		public string LoadedSceneName { get { return SceneManager.GetActiveScene().name; } }

		public event UnityEngine.Events.UnityAction<Scene, LoadSceneMode> OnSceneLoaded
		{
			add	{SceneManager.sceneLoaded += value;	}
			remove	{SceneManager.sceneLoaded -= value;}
		}

		public event UnityEngine.Events.UnityAction<Scene> OnSceneUnloaded
		{
			add { SceneManager.sceneUnloaded += value; }
			remove { SceneManager.sceneUnloaded -= value; }
		}

		public event UnityEngine.Events.UnityAction<Scene, Scene> OnSceneChanaged
		{
			add { SceneManager.activeSceneChanged += value; }
			remove { SceneManager.activeSceneChanged -= value; }
		}


		public void LoadSceneAsync(
					string newSceneName
					, LoadSceneMode mode = LoadSceneMode.Single
					, Action<int> progressActionByPercent = null
					, IAsyncOperation externalLoader = null
					, Action<SceneLoadingInfo> onSceneLoadingDone = null
				)
		{
			SceneLoadingInfo sceneLoadInfoCache = null;
			if (!Dictionary.TryGetValue(newSceneName, out sceneLoadInfoCache))
			{
				sceneLoadInfoCache = ObjectCache.GlobalCache.Allocate<SceneLoadingInfo>();
				sceneLoadInfoCache.mode = mode;
				sceneLoadInfoCache.sceneName = newSceneName;
				sceneLoadInfoCache.progressActionByPercent = progressActionByPercent;
				sceneLoadInfoCache.OnActivated += SceneLoadInfoCache_OnActivated;
				sceneLoadInfoCache.OnSceneLoadingDone += onSceneLoadingDone;
				sceneLoadInfoCache.externalAsyncOperations = externalLoader;//new testLoader();
				StartCoroutine(sceneLoadInfoCache.LoadAsync());
				Dictionary[sceneLoadInfoCache.sceneName] = sceneLoadInfoCache;
			}
		}

		private void SceneLoadInfoCache_OnActivated(SceneLoadingInfo sceneLoadInfo)
		{
			Dictionary.Remove(sceneLoadInfo.sceneName);
			ObjectCache.GlobalCache.Recycle(sceneLoadInfo);
		}




		//加载显示进度的UI界面

		public void InitLoadingUI()
		{

			//UI加载进度界面
			
		}
		Gate gate = new Gate();
		private void OnGUI()
		{
			if (Input.GetKeyDown(KeyCode.M))
			{
				var i = SceneManager.GetSceneAt(0);
				var p = SceneManager.CreateScene("__new");
				new GameObject("test1");
				var m = SceneManager.sceneCount;
				SceneManager.SetActiveScene(p);
				new GameObject("test2");
				var v = SceneManager.GetActiveScene();
				m = SceneManager.sceneCount;
				//SceneManager.LoadSceneAsync(LoadedSceneName);
			}
			if (gate.IsOpen && Input.GetKeyDown(KeyCode.N))
			{
				gate.Close();
				LoadSceneAsync("SYART/Scene/Station1", LoadSceneMode.Additive, i => {
					print("progress:" + i);
				},
				null,
				i => { return; i.Activate(); }
				);
			}
			if (gate.IsClosed && Input.GetKeyDown(KeyCode.P))
			{
				Dictionary["SYART/Scene/Station1"].Activate();
				gate.Open();
			}
		}

	}
}
