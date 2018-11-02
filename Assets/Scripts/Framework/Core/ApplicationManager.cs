using UnityEngine;
using Framework.Core.Attributes;
using Framework.Library.Singleton;
using Framework.Library.Log;
using System.IO;
using Framework.Utils.Extensions;
using System;

namespace Framework.Core
{
	[PathInHierarchy("/[Game]/Application"), DisallowMultipleComponent]
	public sealed class ApplicationManager : ToSingletonBehavior<ApplicationManager>, IApplication
	{
		event Action IApplication.LowMemory
		{
			add
			{
				_LowMemoryCallback += value;
			}

			remove
			{
				_LowMemoryCallback -= value;
			}
		}

		private event Action _LowMemoryCallback;

		event Action IApplication.OnQuit
		{
			add
			{
				_OnAppQuit += value;
			}

			remove
			{
				_OnAppQuit -= value;
			}
		}

		private event Action _OnAppQuit;

		event Func<bool> IApplication.WantsToQuit
		{
			add
			{
				_WantsToQuit += value;
			}

			remove
			{
				_WantsToQuit -= value;
			}
		}

		private event Func<bool> _WantsToQuit;

		private void Awake()
		{
			Debug.Log("AM.Awake");
			//初始化日志
			ApplicationLogger.Instance.Init();
			_helper = ApplicationLogger.Instance;
			Application.lowMemory += delegate ()
			{
				if (_LowMemoryCallback != null)
				{
					_LowMemoryCallback();
				}
			};
#if UNITY_2018_1_OR_NEWER
			//必定执行
			//Application.wantsToQuit == false 时不会执行OnApplicationQuit

			Application.quitting += delegate ()
			{
				if (_OnAppQuit != null)
				{
					_OnAppQuit();
				}
				UnityEngine.Debug.Log("-><-");
			};

			Application.wantsToQuit += delegate ()
			{
				if (_WantsToQuit != null)
				{
					return _WantsToQuit();
				}
				return true;
			};
#else
#endif
		}

#if !UNITY_2018_1_OR_NEWER
		private void OnApplicationQuit()
		{
			if (_OnAppQuit != null)
			{
				_OnAppQuit();
			}
			Debug.Log("OnApplicationQuit");
		}
#endif

		private void OnDestroy()
		{
			Dispose();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			if (_console != null)
			{
				GameConsole.Instance.Dispose();
				_console = null;
			}
			if (_helper != null)
			{
				ApplicationLogger.Instance.Dispose();
				_helper = null;
			}
			Debug.Log("AM.OnDestroy");
		}

		protected override void OnSingletonInit() // after awake called
		{
			base.OnSingletonInit();
		}

		private IAppConfigure itf_configure = null;

		IConsole _console = null;
		IConsole IApplication.Console { get { return _console; } }
		ILoggerHelper _helper = null;

		IAppConfigure IApplication.AppConfigure	{ get { return itf_configure; } }

		ILoggerHelper IApplication.Logger
		{
			get
			{
				return _helper;
			}
		}

		void IApplication.Initialize(IAppConfigure configure)
		{
			itf_configure = configure;
		}

		void IApplication.InitGamePlay(int frameRate)
		{
			Application.targetFrameRate = frameRate;
			GameManager.Instance.Initalize();
			if (_console == null)
			{
				GameConsole.Instance.Initlize();
				_console = GameConsole.Instance;
			}
		}

	}
}