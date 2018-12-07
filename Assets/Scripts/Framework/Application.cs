using Framework.Core;
using Framework.Library.Log;
using Framework.Library.ObjectPool;
using Framework.Library.ObjectPool.Policies;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public interface IApplication
	{
		event Action LowMemory;
		event Action OnQuit;
		event Func<bool> WantsToQuit;
		void Initialize(IAppConfigure configure);
		void InitGamePlay(int frameRate = 60);
		void Exit(int retCode = 0);
		ILoggerHelper Logger { get; }
		IConsole Console { get; }
		IAppConfigure AppConfigure { get; }
		int MainThreadId { get; }
		IGamePlay GamePlay { get; }
	}

	public interface IGamePlay : IDisposable
	{
		void Initalize();
		void Update();

		void AddSubManager<T>() where T : IManager, new();

		void AddSubManager(Type managerType);

		T GetSubManager<T>() where T : class, IManager;

		IManager GetSubManager(Type managerType);

		void RemoveSubManager(Type managerType);

		void RemoveSubManager<T>() where T : IManager;

		IEnumerable<U> GetRelationedObjects<T, U>()
			where T : class, IManager
			where U : class, IManageredObject;

	}

	public interface IMemoryPoolManager : IDisposable
	{
		IObjectPool<TObject> CreatePool<TObject>(string name, IObjectFactory<TObject> factory = null) where TObject : class;
		IObjectPool<TObject> CreatePool<TObject, TFactory, TBufferPolicy>(string name)
			where TFactory : IObjectFactory<TObject>, new()
			where TBufferPolicy : BufferPolicy<TObject>
			where TObject : class;
		IObjectPool<TObject> CreatePool<TObject, TBufferPolicy>(string name)
			where TBufferPolicy : BufferPolicy<TObject>
			where TObject : class;
		IObjectPool<T> GetObjectPool<T>(string name) where T : class;
		void ReleaseAllUnusedPoolableObjects();
		void ReleasePool<TObject>(string name) where TObject : class;
		IObjectPool<TObject> CreateSharedPool<TObject>(string name, IObjectFactory<TObject> factory = null) where TObject : class;
		IObjectPool<TObject> CreateSharedPool<TObject, TFactory>(string name)
			where TObject : class
			where TFactory : IObjectFactory<TObject>, new();
	}

	public interface IConsole : IDisposable
	{
		event Action<string> OnText;
		void Register(char PrefixChar, string cmdText, Action<string[]> commandProcessor);
		void Unregister(char PrefixChar, string cmdText);
	}

	public interface ILoggerHelper
	{
		LogUtil.ILogHelper LogHelper { get; }
	}

	public static partial class FrameworkUtils
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Init()
		{
			//Initialize Application
			Application = ApplicationManager.Instance;
			ApplicationManager.Instance.OnEventDispose += delegate { Application = null; };
			Application.LowMemory += delegate () { };
			Application.WantsToQuit += delegate () { return true; };
			Application.OnQuit += delegate () { };
			Application.Initialize(new AppConfigure());
			Application.InitGamePlay();
		}

		public static IApplication Application 	{ get; private set;	}

		public static IGamePlay GamePlay { get {return Application.GamePlay; } }
		public static IMemoryPoolManager MemoryPoolManager { get; private set; }

		static FrameworkUtils()
		{
			MemoryPoolManager = new MemoryPoolManager();
		}
	}

}
