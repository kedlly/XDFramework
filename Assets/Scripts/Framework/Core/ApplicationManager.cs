using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;
using System;

namespace Framework.Core
{
	[PathInHierarchy("/[Game]/Application"), DisallowMultipleComponent]
	public sealed class ApplicationManager : ToSingletonBehavior<ApplicationManager>, IApplication
	{
		public event Action LowMemory;

		public event Action OnQuit;

		public event Func<bool> WantsToQuit;

		private int _mainThreadId = -1;
		private void Awake()
		{
			Debug.Log("ApplicationManager.Awake");
			_mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			//初始化日志
			Application.lowMemory += delegate ()
			{
				if (LowMemory != null)
				{
					LowMemory();
				}
			};
#if UNITY_2018_1_OR_NEWER
			//必定执行
			//Application.wantsToQuit == false 时不会执行OnApplicationQuit

			Application.quitting += delegate ()
			{
				if (OnQuit != null)
				{
					OnQuit();
				}
				UnityEngine.Debug.Log("-><-");
			};

			Application.wantsToQuit += CheckQuitCondition;
#else
#endif
		}

#if !UNITY_2018_1_OR_NEWER
		private void OnApplicationQuit()
		{
			if (OnQuit != null)
			{
				OnQuit();
			}
			Debug.Log("OnApplicationQuit");
		}
#endif

		private bool CheckQuitCondition()
		{
			if (WantsToQuit != null)
			{
				return WantsToQuit();
			}
			return true;
		}

		private void OnDestroy()
		{
			Dispose();
			Destroy(this.gameObject);
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			ApplicationLogger.Instance.Dispose();
			if (Console != null)
			{
				Console.Dispose();
				Destroy(Console as GameConsole);
				Console = null;
			}
			AppConfigure = null;
			GamePlay.Dispose();
			Debug.Log("ApplicationManager.OnDestroy");
		}

		protected override void OnSingletonInit() // after awake called
		{
			base.OnSingletonInit();
		}


		public IConsole Console { get; internal set; }

		public IAppConfigure AppConfigure	{ get; private set; }

		ILoggerHelper IApplication.Logger	{	get	{	return ApplicationLogger.Instance;	}	}

		public int MainThreadId
		{
			get
			{
				return _mainThreadId;
			}
		}

		void IApplication.Initialize(IAppConfigure configure)
		{
			AppConfigure = configure;
			//Initialize Application's Logger
			ApplicationLogger.Instance.Init(AppConfigure.LogPath);
			Console = this.gameObject.AddComponent<GameConsole>();
			(Console as GameConsole).Initlize();
		}

		void IApplication.InitGamePlay(int frameRate)
		{
			Application.targetFrameRate = frameRate;
			GamePlay = new GamePlay();
			GamePlay.Initalize();
		}

		public IGamePlay GamePlay	{ get; private set; }

		private void Update()
		{
			if (GamePlay != null)
			{
				GamePlay.Update();
			}
		}

		public void Exit(int retCode = 0)
		{

#if !UNITY_2018_1_OR_NEWER
			if (!CheckQuitCondition())
			{
				return;
			}
#endif

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}