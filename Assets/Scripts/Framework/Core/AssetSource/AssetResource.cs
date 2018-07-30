using Framework.Core.FlowControl;
using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Core
{
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
		AssetBundleCreateRequest abcq;
		AssetBundleRequest abq;
		bool isLoading = false;

		Gate abcqGate = new Gate(true);
		Gate abqGate = new Gate(true);

		const float FileLoadPercentage = 0.35f;

		public AssetResourceFromABPath() : base()
		{
			abcqGate.OnExit += () =>
			{
				this.Progress = abcq.progress * FileLoadPercentage;
				if (abcq.isDone)
				{
					abcqGate.Close();
					bundle = abcq.assetBundle;
					if (bundle == null)
					{
						IsDone = true;
						OnLoadingEvent_Failed(this, "{0} / {1} bundle error".FormatEx(AssetBundlePath, AssetName));
						return;
					}
					abq = bundle.LoadAssetAsync<T>(AssetName);
					abqGate.Open();
				}
			};
			abqGate.OnExit += () =>
			{
				this.Progress = abq.progress * (1 - FileLoadPercentage) + FileLoadPercentage;
				if (abq.isDone)
				{
					abqGate.Close();
					Asset = new T[1] { abq.asset as T };
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
			abcq = AssetBundle.LoadFromFileAsync(AssetBundlePath);
			abcqGate.Open();
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
				abcqGate.Execute();
				abqGate.Execute();
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

	public sealed class AssetBundleLoader : IABOperation
	{
		public string AssetBundlePath { get; private set; }

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

		public AssetBundleLoader(string abPath)
		{
			AssetBundlePath = abPath;
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
			if (_state == OperationState.Running || _state == OperationState.Done)
			{
				return;
			}
			_state = OperationState.Running;
			AssetLoaderProxy = null;
			_progress = 0f;
			abcq = AssetBundle.LoadFromFileAsync(AssetBundlePath);
			abcqGate.Open();
		}

		public void LoadSync()
		{
			if (_state == OperationState.Running || _state == OperationState.Done)
			{
				return;
			}
			var bundle = AssetBundle.LoadFromFile(AssetBundlePath);
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
	}

}
