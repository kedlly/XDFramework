using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public interface IAppConfigure
	{
		string ServerURL { get; }
		string Copyright { get; }
		string Version { get; }
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
	}
}
