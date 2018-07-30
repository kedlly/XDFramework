using UnityEngine;
using Framework.Library.Singleton;
using Framework.Library.Log;
using System.IO;

namespace Framework.Core
{

	static partial class RuntimeInitializeBeforeSceneLoad
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void InitApplicationManager()
		{
			ApplicationManager.Instance.initalizeUnity();
			ApplicationManager.Instance.InitGame();
		}
	}

	[GameObjectPath("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class ApplicationManager : ToSingletonBehavior<ApplicationManager>
	{
		private class ApplicationLogger : ToSingleton<ApplicationLogger>
		{
#if UNITY_EDITOR
			string mDevicePersistentPath = Application.dataPath + "/../PersistentPath";
#elif UNITY_STANDALONE_WIN
			string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
#elif UNITY_STANDALONE_OSX
			string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
#else
			string mDevicePersistentPath = Application.persistentDataPath;
#endif

			const string LogPath = "Logs";

			public LogUtil.ILogHelper LogHelper { get; private set; }

			private ApplicationLogger()
			{

			}

			public void Init()
			{
				LogHelper = new FileLogOutput(Path.Combine(mDevicePersistentPath, LogPath));
				Application.logMessageReceivedThreaded += LogCallback;
				LogUtil.SetLogHelper(LogHelper);
			}

			void LogCallback(string condition, string stackTrace, LogType type)
			{
				if (LogHelper == null)
				{
					return;
				}
				LogHelper.Log(
					new LogData
					{
						Message = condition
						, StackTrace = stackTrace
						, Level = type
					}
				);
			}

			protected override void OnSingletonInit()
			{
				
			}

			protected override void OnDispose()
			{
				LogUtil.SetLogHelper(null);
				Application.logMessageReceivedThreaded -= LogCallback;
				if(LogHelper != null)
				{
					LogHelper.CleanAllHandle();
					LogHelper.Close();
					LogHelper = null;
				}
				base.OnDispose();
			}
		}

		private void Awake()
		{
			//初始化日志
			ApplicationLogger.Instance.Init();
		}

		private void OnDestroy()
		{
			Dispose();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			ApplicationLogger.Instance.Dispose();
		}

		protected override void OnSingletonInit() // after awake called
		{
			base.OnSingletonInit();
		}

		public void initalizeUnity()
		{
			//Application.backgroundLoadingPriority = ThreadPriority.High;
			Application.lowMemory += delegate ()
			{

			};

			//必定执行
			//Application.wantsToQuit == false 时不会执行OnApplicationQuit
			Application.quitting += delegate ()
			{
				UnityEngine.Debug.Log("-><-");
			};

			Application.wantsToQuit += delegate ()
			{
				return false;
			};
			
		}

		private void OnApplicationQuit() {
			Debug.Log("OnApplicationQuit");
		}

		public void InitGame()
		{
			GameManager.Instance.Initalize();
			//GameConsole.Instance.Initlize();
			/*
			GameObject o = null;
			o.AddSubObject("A");
			o.AddSubObject("A");
			o.AddSubObject("A");
			var q = o.AddSubObject("A/B/C/D").AddSubObject("O/P/Q");q.AddSubObject("b");
			DontDestroyOnLoad(o.AddSubObject("A"));
			o.AddSubObject("A/E/F/C");
			o.AddSubObject("A/E/Q/C");*/
			
		}
	}
}