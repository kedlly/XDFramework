using Framework.Core.FlowControl;
using Framework.Library.ObjectPool;
using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Framework.Core
{
	public interface IAssetBundle<out T> where T : UObject
	{
		bool IsDependenciesAllLoaded { get; }
		T GetResource();
		T GetResource(string name);
	}

	


	public class AssetBundleCluster : IPoolable
	{
		public string TagName { get; private set; }
		private bool allocFromPool { get; set; }
		Dictionary<string, AssetBundle> cacheDict = new Dictionary<string, AssetBundle>(8);
		public void AddRequiredAssetBundle(string abPath, string uri)
		{
			cacheDict.Add(abPath, null);
		}
		public void LoadRequiredAssetBundles(bool isAsync = true)
		{
			if (isAsync)
			{
				LoadAsync();
			}
			else
			{
				Load();
			}
		}


		private AssetBundleCreateRequest abcr;
		private void LoadAsync()
		{
			//WWW.LoadFromCacheOrDownload()
			//abcr = AssetBundle.LoadFromFileAsync()
			
		}

		private void Load()
		{

		}

		public void OnAllocated()
		{
			//AssetBundle.GetAllLoadedAssetBundles();
		}

		public void OnRecycled()
		{
			cacheDict.Clear();
			TagName = null;
		}

		public void SetTagName(string name)
		{
			TagName = name;
		}

		public void Unload(bool force = false) { }
		public static AssetBundleCluster Allocate()
		{
			var obj = ObjectCache.GlobalCache.Allocate<AssetBundleCluster>();
			obj.allocFromPool = true;
			return obj;
		}
		public void Recycle()
		{
			if (this.allocFromPool)
			{
				Recycle(this);
			}
			else
			{
				OnRecycled();
			}
		}

		public static void Recycle(AssetBundleCluster abc)
		{
			if (abc != null && abc.allocFromPool)
			{
				ObjectCache.GlobalCache.Recycle(abc);
			}
		}
	}


	
	public abstract class AssetResource<T> where T : UnityEngine.Object
	{
		public T[] Asset { get; protected set; }
		public bool IsDone { get; protected set; }
		public float Progress { get; protected set; }
		public virtual void Check() { }
		public virtual void LoadAsync() { }
		public virtual void LoadSync() { }
		public AssetResource()
		{
			IsDone = false;
			Progress = 0f;
			Asset = null;
		}

		public event Action<AssetResource<T>, string> OnLoadingSucceed;
		public event Action<AssetResource<T>, string> OnLoadingFailed;

		protected void OnLoadingEvent_Succeed(AssetResource<T> ar, string message)
		{
			if (OnLoadingSucceed != null)
			{
				OnLoadingSucceed(ar, message);
			}
		}

		protected void OnLoadingEvent_Failed(AssetResource<T> ar, string message)
		{
			if (OnLoadingFailed != null)
			{
				OnLoadingFailed(ar, message);
			}
		}

		public virtual void UnloadAllLoadedObjects() { }
		public virtual void Unload() { }
	}

	public class AssetResourceFromABPath<T> : AssetResource<T> where T : UnityEngine.Object
	{
		public string AssetBundlePath { get; set; }
		public string AssetName { get; set; }
		AssetBundle bundle;
		AssetBundleCreateRequest abcr;
		AssetBundleRequest abr;
		bool isLoading = false;

		Gate abcrGate = new Gate(true);
		Gate abrGate = new Gate(true);

		const float FileLoadPercentage = 0.35f;

		public AssetResourceFromABPath() : base()
		{
			abcrGate.OnExit += () =>
			{
				this.Progress = abcr.progress * FileLoadPercentage;
				if (abcr.isDone)
				{
					abcrGate.Close();
					bundle = abcr.assetBundle;
					if (bundle == null)
					{
						IsDone = true;
						OnLoadingEvent_Failed(this, "{0} / {1} bundle error".FormatEx(AssetBundlePath, AssetName));
						return;
					}
					abr = bundle.LoadAssetAsync<T>(AssetName);
					abrGate.Open();
				}
			};
			abrGate.OnExit += () =>
			{
				this.Progress = abr.progress * (1 - FileLoadPercentage) + FileLoadPercentage;
				if (abr.isDone)
				{
					abrGate.Close();
					Asset = new T[1] { abr.asset as T };
					IsDone = true;
					if (Asset == null || Asset[0] == null)
					{
						OnLoadingEvent_Failed(this, "{0} / {1} / {2} bundle error, type is".FormatEx(AssetBundlePath, AssetName, typeof(T).Name));
					}
					else
					{
						OnLoadingEvent_Succeed(this, null);
					}
					isLoading = false;
				}
			};
		}

		public override void LoadAsync()
		{
			if (IsDone)
			{
				return;
			}
			isLoading = true;
			bundle = null;
			abcr = AssetBundle.LoadFromFileAsync(AssetBundlePath);
			abcrGate.Open();
		}

		public override void LoadSync()
		{
			if (IsDone)
			{
				return;
			}
			bundle = AssetBundle.LoadFromFile(AssetBundlePath);
			Asset = new T[1] { bundle.LoadAsset<T>(AssetName) };
			IsDone = true;
			Progress = 1f;
		}

		public override void Check()
		{
			if (isLoading)
			{
				abcrGate.Execute();
				abrGate.Execute();
			}
		}

		public override void Unload()
		{
			base.Unload();
			if (bundle != null)
			{
				bundle.Unload(false);
				bundle = null;
			}
		}

		public override void UnloadAllLoadedObjects()
		{
			base.UnloadAllLoadedObjects();
			if (bundle != null)
			{
				bundle.Unload(true);
				bundle = null;
			}
			this.Asset = null;
		}
	}

	public enum OperationState
	{
		Ready, Running, Done, Abort
	}

	public interface IABOperation
	{
		void LoadSync();
		void LoadAsync();
		void CheckOperation();
		float Progress { get; }
		OperationState State {get;}
	}


    public sealed class AssetLoader
	{
		const string C_AssetBundleManifest = "AssetBundleManifest";
		Dictionary<string, UnityEngine.Object> AssetBuffer = new Dictionary<string, UnityEngine.Object>();

		AssetBundle bundle;

		public float Progress { get { return _progress; } }

		public OperationState State
		{
			get
			{
				return _state;
			}
		}

		List<AssetLoaderStep> prepareLoadingSteps = new List<AssetLoaderStep>();
		List<AssetLoaderStep> currentLoadingSteps = new List<AssetLoaderStep>();

		public static float MaxLoadingCount = 3;


		OperationState _state = OperationState.Ready;
		float _progress = 0f;

		internal class AssetLoaderStep
		{
			public OperationState State
			{
				get
				{
					return _state;
				}
			}
			Action<AssetBundleRequest> theAction;
			AssetBundle bundle;
			AssetBundleRequest abr = null;
			OperationState _state = OperationState.Ready;
			float _progress = 0f;
			string assetName = "";

			public AssetLoaderStep(AssetBundle bundle, string name, Action <AssetBundleRequest> action)
			{
				this.bundle = bundle;
				assetName = name;
				theAction = action;
			}

			public void CheckOperation()
			{
				if (_state == OperationState.Running )
				{
					_progress = abr.progress;
					if (abr.isDone)
					{
						if (abr.asset != null)
						{
							_state = OperationState.Done;
							if (theAction != null)
							{
								theAction(abr);
							}
						}
						else
						{
							_state = OperationState.Abort;
						}
					}
				}
			}

			public void Run()
			{
				abr = bundle.LoadAssetAsync(assetName);
				if (abr != null)
				{
					_state = OperationState.Running;
				}
				else
				{
					_state = OperationState.Abort;
				}
			}
		}
		

		public AssetLoader(AssetBundle ab)
		{
			bundle = ab;
		}

		public void LoadAssetSync(string name)
		{
			if (AssetBuffer.ContainsKey(name))
			{
				return;
			}
			var asset = bundle.LoadAsset(name);
			AssetBuffer.Add(asset.name, asset);
		}

		public T GetAsset<T>(string name) where T : UnityEngine.Object
		{
			if (AssetBuffer.ContainsKey(name))
			{
				return AssetBuffer[name] as T;
			}
			return null;
		}

		public void LoadAssetAsync(string name) 
		{
			if (_state == OperationState.Running || AssetBuffer.ContainsKey(name))
			{
				return;
			}
			AssetLoaderStep newStep = new AssetLoaderStep(bundle, name, abr => 
			{
				if (abr.asset != null)
				{
					AssetBuffer.Add(abr.asset.name, abr.asset);
				}
				else
				{
					Debug.LogWarningFormat("can not load asset with name :{0} in assetbundle : {1}".FormatEx(name, bundle.name));
				}
			});
			prepareLoadingSteps.Add(newStep);
		}

		public void CheckOperation()
		{
			if (prepareLoadingSteps.Count > 0)
			{
				foreach (var loadingProcess in prepareLoadingSteps)
				{
					if (loadingProcess.State == OperationState.Ready)
					{
						loadingProcess.Run();
					}
					else
					{
						if (loadingProcess.State == OperationState.Running)
						{
							loadingProcess.CheckOperation();
						}
					}
				}
			}

		}
	}

	public sealed class AssetBundleLoader : IABOperation, IPoolable
	{
		
		public float Progress { get { return _progress; } }
		
		public OperationState State
		{
			get
			{
				return _state;
			}
		}

		OperationState _state = OperationState.Ready;
		float _progress = 0f;
		Gate abcqGate = new Gate(true);

		AssetBundleCreateRequest abcq;
		public AssetLoader AssetLoaderProxy { get; private set; }
		public string TagName { get; private set; }


		private bool allocFromPool { get; set; }
		private string URI { get; set; }
		private string[] PreLoadResources = null;
		private WWW wwwObject = null;



		public void SetResource(string uri, params string[] preloads)
		{
			if (uri.Contains("://"))
			{
				//wwwObject = new WWW(uri);
				//wwwObject.Dispose();
			}
			PreLoadResources = preloads;
		}

		public AssetBundleLoader(string abPath)
		{
			abcqGate.OnExit += () =>
			{
				_progress = abcq.progress;
				if (abcq.isDone)
				{
					abcqGate.Close();
					var bundle = abcq.assetBundle;
					if (bundle == null)
					{
						_state = OperationState.Abort;
					}
					else
					{
						AssetLoaderProxy = new AssetLoader(bundle);
						_state = OperationState.Done;
					}
				}
			};
		}

		public void LoadAsync()
		{
			if (_state == OperationState.Running || _state == OperationState.Done || URI.IsNullOrEmpty())
			{
				return;
			}
			_state = OperationState.Running;
			AssetLoaderProxy = null;
			_progress = 0f;
			if (URI.Contains("://"))
			{
				wwwObject = new WWW(URI);
				//wwwObject.Dispose();
			}
			abcq = AssetBundle.LoadFromFileAsync(URI);
			abcqGate.Open();
		}

		public void LoadSync()
		{
			if (_state == OperationState.Running || _state == OperationState.Done)
			{
				return;
			}
			var bundle = AssetBundle.LoadFromFile(URI);
			if (bundle == null)
			{
				_state = OperationState.Abort;
				_progress = 0f;
			}
			else
			{
				_state = OperationState.Done;
				_progress = 1f;
				AssetLoaderProxy = new AssetLoader(bundle);
			}
			
		}

		public void CheckOperation()
		{
			if (OperationState.Running == _state)
			{
				abcqGate.Execute();
			}
			if (AssetLoaderProxy != null)
			{
				AssetLoaderProxy.CheckOperation();
			}
		}

		public static AssetBundleLoader Allocate()
		{
			var obj = ObjectCache.GlobalCache.Allocate<AssetBundleLoader>();
			obj.allocFromPool = true;
			obj.URI = null;
			return obj;
		}
		public void Recycle()
		{
			if (this.allocFromPool)
			{
				Recycle(this);
			}
			else
			{
				OnRecycled();
			}
		}

		public static void Recycle(AssetBundleLoader abc)
		{
			if (abc != null && abc.allocFromPool)
			{
				ObjectCache.GlobalCache.Recycle(abc);
			}
		}

		public void OnAllocated()
		{
			//AssetBundle.GetAllLoadedAssetBundles();
		}

		public void OnRecycled()
		{
			TagName = null;
		}

		public void SetTagName(string name)
		{
			TagName = name;
		}
	}

}
