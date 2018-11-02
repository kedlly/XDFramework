using Framework.Library.Log;
using Framework.Library.Singleton;
using System.IO;
using UnityEngine;

namespace Framework.Core
{
	public class ApplicationLogger : ToSingleton<ApplicationLogger>, ILoggerHelper
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
					,
					StackTrace = stackTrace
					,
					Level = type
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
