using Framework.Core;
using Framework.Library.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		ILoggerHelper Logger { get; }
		IConsole Console { get; }
		IAppConfigure AppConfigure { get; }
	}
	

	public interface IConsole
	{
		event Action<string> OnText;
		void Register(char PrefixChar, string cmdText, Action<string[]> commandProcessor);
		void Unregister(char PrefixChar, string cmdText);
	}

	public interface ILoggerHelper
	{
		LogUtil.ILogHelper LogHelper { get; }
	}

	


	public static partial class Framework
	{
		static IApplication itf_Application = null;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void InitApplication()
		{
			itf_Application = ApplicationManager.Instance;
			itf_Application.LowMemory += delegate () { };
			itf_Application.WantsToQuit += delegate () { return true; };
			itf_Application.OnQuit += delegate () { };
			itf_Application.Initialize(new AppConfigure());
			itf_Application.InitGamePlay();
		}
		
		public static IApplication Application
		{
			get { return itf_Application; }
		}
	}

}
