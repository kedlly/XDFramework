
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Framework.Library.Configure
{
	public class IniFile
	{
		public string Path;
		string EXE = Assembly.GetExecutingAssembly().GetName().Name;
#if UNITY_STANDALONE_WIN
		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
#elif UNITY_STANDALONE_OSX
		static long WritePrivateProfileString(string Section, string Key, string Value, string FilePath)
		{
			return 0;
		}
		static int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath)
		{
			return 0;
		}
#else 
#endif
		public IniFile(string IniPath = null)
		{
			Path = new FileInfo(IniPath == null ? EXE + ".ini" : IniPath).FullName.ToString();
		}

		public string Read(string Key, string Section = null)
		{
			var RetVal = new StringBuilder(255);
			GetPrivateProfileString(Section == null ? EXE : Section, Key, "", RetVal, 255, Path);
			return RetVal.ToString();
		}

		public void Write(string Key, string Value, string Section = null)
		{
			WritePrivateProfileString(Section == null ? EXE : Section, Key, Value, Path);
		}

		public void DeleteKey(string Key, string Section = null)
		{
			Write(Key, null, Section == null ? EXE : Section);
		}

		public void DeleteSection(string Section = null)
		{
			Write(null, null, Section == null ? EXE : Section);
		}

		public bool KeyExists(string Key, string Section = null)
		{
			return Read(Key, Section).Length > 0;
		}
	}
}