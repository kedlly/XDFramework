using UnityEngine;


namespace Framework.Library.Log
{

	public class LogData
	{
		public string Message { get; set; }
		public string StackTrace { get; set; }
		public LogType Level { get; set; }
	}

	public static partial class LogUtil
    {

		public delegate void HandleLog(LogData logData);

        /// <summary>
        /// 日志辅助器接口。
        /// </summary>
        public interface ILogHelper
        {
            /// <summary>
            /// 记录日志。
            /// </summary>
            /// <param name="level">日志等级。</param>
            /// <param name="message">日志内容。</param>
            void Log(LogData logData);
			/// <summary>
			/// 关闭日志
			/// </summary>
			void Close();

			event HandleLog handleLog;

			void CleanAllHandle();

		}
    }
}
