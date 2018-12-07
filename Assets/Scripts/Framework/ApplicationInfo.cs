
using Framework.Misc.Paths;

namespace Framework
{
	public interface IAppConfigure
	{
		string ServerURL { get; }
		string Copyright { get; }
		string Version { get; }
		string LogPath { get; }
		string[] AppArgs { get; }
	}

	class AppConfigure : IAppConfigure
	{
		string IAppConfigure.ServerURL
		{
			get
			{
				return "http://www.wuhanxindian.com";
			}
		}

		string IAppConfigure.Copyright { get { return "http://www.wuhanxindian.com"; } }

		string IAppConfigure.Version { get { return "0.0.0.1"; } }

		string[] IAppConfigure.AppArgs
		{
			get
			{
				return System.Environment.GetCommandLineArgs();
			}
		}

		const string C_LogPath = "Logs";
		private string _logPath = System.IO.Path.Combine(DevicePath.PersistentDataPath, C_LogPath);
		string IAppConfigure.LogPath
		{
			get
			{
				return _logPath;
			}
		}
	}
}
