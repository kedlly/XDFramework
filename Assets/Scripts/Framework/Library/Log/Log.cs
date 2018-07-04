﻿

using System.Diagnostics;
using UnityEngine;

namespace Framework.Library.Log
{
    /// <summary>
    /// 日志类。
    /// </summary>
    public static partial class LogUtil
	{
        private static ILogHelper s_LogHelper = null;

        /// <summary>
        /// 设置日志辅助器。
        /// </summary>
        /// <param name="logHelper">要设置的日志辅助器。</param>
        public static void SetLogHelper(ILogHelper logHelper)
        {
            s_LogHelper = logHelper;
        }

		/// <summary>
		/// 取得日志辅助器
		/// </summary>
		/// <returns></returns>
		public static ILogHelper GetLogHelper()
		{
			return s_LogHelper;
		}

		private static LogData getLogData(LogType type, object message)
		{
			return new LogData
			{
				Message = message.ToString(),
				StackTrace = "",
				Level = type
			};
		}

		private static LogData getLogData(LogType type, string message)
		{
			return new LogData
			{
				Message = message,
				StackTrace = "",
				Level = type
			};
		}

		/// <summary>
		/// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
		/// </summary>
		/// <param name="message">日志内容。</param>
		[Conditional("DEBUG")]
        public static void Debug(object message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, message));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="message">日志内容。</param>
        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, message));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, arg0)));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, arg0, arg1)));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, object arg0, object arg1, object arg2)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, arg0, arg1, arg2)));
        }

        /// <summary>
        /// 记录调试级别日志，仅在带有 DEBUG 预编译选项时产生。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, args)));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, message));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, message));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Info(string format, object arg0)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, arg0)));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Info(string format, object arg0, object arg1)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, arg0, arg1)));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Info(string format, object arg0, object arg1, object arg2)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log,  string.Format(format, arg0, arg1, arg2)));
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Info(string format, params object[] args)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Log, string.Format(format, args)));
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Warning(object message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Warning , message));
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Warning(string message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Warning, message));
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Warning(string format, object arg0)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Warning, string.Format(format, arg0)));
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Warning(string format, object arg0, object arg1)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Warning, string.Format(format, arg0, arg1)));
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Warning(string format, object arg0, object arg1, object arg2)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Warning, string.Format(format, arg0, arg1, arg2)));
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Warning(string format, params object[] args)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Warning, string.Format(format, args)));
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Error(object message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Error, message));
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public static void Error(string message)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Error, message));
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        public static void Error(string format, object arg0)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Error, string.Format(format, arg0)));
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        public static void Error(string format, object arg0, object arg1)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Error, string.Format(format, arg0, arg1)));
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <param name="arg2">日志参数 2。</param>
        public static void Error(string format, object arg0, object arg1, object arg2)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Error, string.Format(format, arg0, arg1, arg2)));
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="args">日志参数。</param>
        public static void Error(string format, params object[] args)
        {
            if (s_LogHelper == null)
            {
                return;
            }

            s_LogHelper.Log(getLogData(LogType.Error, string.Format(format, args)));
        }
    }
}
