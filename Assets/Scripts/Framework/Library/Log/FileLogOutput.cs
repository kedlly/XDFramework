using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using Framework.Utils.Extensions;

namespace Framework.Library.Log
{
	public class FileLogOutput : LogUtil.ILogHelper
	{

		private object threadLock = new object();
		private Thread logThread = null;
		public bool isLoggerEnable { get; set; }
		private StreamWriter logWriter = null;

		public event LogUtil.HandleLog handleLog;


		public FileLogOutput(string pathToLogDirectory)
		{
			logThread = new Thread(WriteLog);
			string logName = string.Format("{0:yyyyMMddHHmmss}.log", DateTime.Now);
			string logPath = Path.Combine(pathToLogDirectory, logName);

			if(File.Exists(logPath))
			{
				File.Delete(logPath);
			}
			string logDir = Path.GetDirectoryName(logPath);
			if(!Directory.Exists(logDir))
			{
				Directory.CreateDirectory(logDir);
			}
			this.logWriter = new StreamWriter(logPath);
			this.logWriter.AutoFlush = true;
			this.isLoggerEnable = true;
			this.logThread.Start();
		}



		private Queue<LogData> writingLogQueue = new Queue<LogData>();
		private Queue<LogData> waitingLogQueue = new Queue<LogData>();

		void WriteLog()
		{
			while(this.isLoggerEnable)
			{
				if(this.writingLogQueue.Count == 0)
				{
					lock(this.threadLock)
					{
						while(this.waitingLogQueue.Count == 0)
							Monitor.Wait(this.threadLock);
						waitingLogQueue = Interlocked.Exchange(ref writingLogQueue, waitingLogQueue);
					}
				}
				else
				{
					while(this.writingLogQueue.Count > 0)
					{
						LogData log = this.writingLogQueue.Dequeue();
						string[] logMsgs = toLogMessage(log);
						logMsgs.ForEach(
							delegate (string msg)
							{
								this.logWriter.WriteLine(msg);
							}
						);
					}
				}
			}
		}

		private const string _SplitLine = "---------------------------------------------------------------------------------------------------------------------";
		private const string _DataContexFormat = "[{0:HH:mm:ss}]\t[{1}]\t";
		private const string _StackTracePrefix = "\t\t";
		private static string[] toLogMessage(LogData logData)
		{
			List<string> messages = new List<string>();
			StringBuilder logMessage = string.Format(_DataContexFormat, DateTime.Now, logData.Level.ToString()).Append(logData.Message);
			if(LogType.Log == logData.Level)
			{
				messages.Add(logMessage.ToString());
			}
			else
			{
				messages.Add(_SplitLine);
				messages.Add(logMessage.ToString());
				if(logData.StackTrace.IsNotNullAndEmpty())
				{
					messages.Add(logData.StackTrace.AddPrefix(_StackTracePrefix));
				}
				messages.Add(_SplitLine);
			}
			return messages.ToArray();
		}

		void LogUtil.ILogHelper.Close()
		{
			this.isLoggerEnable = false;
			this.logWriter.Close();
		}

		public void AddLog(LogData logData)
		{
			lock(this.threadLock)
			{
				this.waitingLogQueue.Enqueue(logData);
				Monitor.Pulse(this.threadLock);
			}
		}

		
		void LogUtil.ILogHelper.Log(LogData logData)
		{
			this.AddLog(logData);
			if (handleLog != null)
			{
				handleLog(logData);
			}
		}

		void LogUtil.ILogHelper.CleanAllHandle()
		{
			if (handleLog != null)
			{
				foreach(Delegate d in handleLog.GetInvocationList())
				{
					handleLog -= (LogUtil.HandleLog)d;
				}
			}
		}
	}
}
