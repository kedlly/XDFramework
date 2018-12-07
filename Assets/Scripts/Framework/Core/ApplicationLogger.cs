using Framework.Library.Log;
using Framework.Library.Singleton;
using System.IO;
using UnityEngine;

namespace Framework.Core
{
	public class ApplicationLogger : ToSingleton<ApplicationLogger>, ILoggerHelper
	{

		public LogUtil.ILogHelper LogHelper { get; private set; }

		private ApplicationLogger()	{}

		public void Init(string path)
		{
			LogHelper = new FileLogOutput(path);
			Application.logMessageReceivedThreaded += LogCallback;
			LogUtil.SetLogHelper(LogHelper);
		}

		void LogCallback(string condition, string stackTrace, LogType type)
		{
			if (LogHelper == null) 	{	return;	}
			LogHelper.Log(
				new LogData
				{
					Message = condition
					, StackTrace = stackTrace
					, Level = type
				}
			);
		}

		protected override void OnSingletonInit(){}

		protected override void OnDispose()
		{
			LogUtil.SetLogHelper(null);
			Application.logMessageReceivedThreaded -= LogCallback;
			if (LogHelper != null)
			{
				LogHelper.CleanAllHandle();
				LogHelper.Close();
				LogHelper = null;
			}
			base.OnDispose();
		}
	}
}
