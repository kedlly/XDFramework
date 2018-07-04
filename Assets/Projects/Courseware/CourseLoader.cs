using Framework.Library.Configure;
using Framework.Utils.Extensions;
using Projects.DataStruct.Courseware;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Projects.Course
{

	internal static class CourseIniKeys
	{
		internal const string SectionSettings = "配置";
		internal const string KeyMode = "模式";							internal const string ValueMode_Editor = "Editor";
		internal const string KeyResourceRelativePath = "资源相对目录";	internal const string ValueResource = "./资源/";
		internal const string KeyCoursewareName = "默认装载课件";
	}
	public static class CourseLoader
	{
		public static bool IsEditorMode { get; set; }
		public static string ResourceDirectory { get; private set; }
		//public string CurrentCoursewareFolderName { get; private set; }

		//课件的配置文件名
		const string CoursewareFileName = "config.json";

		private static IniFile configFile = new IniFile();

		static CourseLoader()
		{
			var mode = configFile.Read(CourseIniKeys.KeyMode, CourseIniKeys.SectionSettings);
			IsEditorMode = (mode == CourseIniKeys.ValueMode_Editor);
			configFile.Write(CourseIniKeys.KeyMode, IsEditorMode ? CourseIniKeys.ValueMode_Editor : "", CourseIniKeys.SectionSettings);
			var relativePath = configFile.Read(CourseIniKeys.KeyResourceRelativePath, CourseIniKeys.SectionSettings);
			if(relativePath.IsNullOrEmpty())
			{
				relativePath = CourseIniKeys.ValueResource;
				configFile.Write(CourseIniKeys.KeyResourceRelativePath, relativePath, CourseIniKeys.SectionSettings);
			}
			ResourceDirectory = Path.Combine(Application.dataPath, relativePath);
			if(!Directory.Exists(ResourceDirectory))
			{
				Directory.CreateDirectory(ResourceDirectory);
			}

			//CurrentCoursewareFolderName = configFile.Read(CourseIniKeys.KeyCoursewareName, CourseIniKeys.SectionSettings);
		}

		public static void Save()
		{
			//if(CurrentCoursewareFolderName.IsNotNullAndEmpty())
			{
				//configFile.Write(CourseIniKeys.KeyCoursewareName, CurrentCoursewareFolderName, CourseIniKeys.SectionSettings);
			}
		}

		public static Courseware NewCourseware(string name)
		{
			Courseware courseware = null;
			if (name.IsNullOrEmpty())
			{
				Debug.LogError("New Courseware must be have a name");
			}
			else
			{
				if(GetAllCoursewares().Contains(name))
				{
					Debug.Log("New Courseware named: {0} exist.".FormatEx(name));
				}
				else
				{
					courseware = new Courseware();
					courseware.Title = name;
				}
			}
			
			return courseware;
		}

		public static string[] GetAllCoursewares()
		{
			if (!Directory.Exists(ResourceDirectory))
			{
				Directory.CreateDirectory(ResourceDirectory);
			}
			DirectoryInfo rd = new DirectoryInfo(ResourceDirectory);
			return rd.GetDirectories().Select(d=>d.Name).ToArray();
		}

		public static Courseware OpenCourseware(string name)
		{
			if (name.IsNullOrEmpty())
			{
				Debug.LogError("Open Courseware must be have a name");
				return null;
			}
			
			var path = Path.Combine(ResourceDirectory, name);
			path = Path.Combine(path, CoursewareFileName);
			if(File.Exists(path))
			{
				StreamReader sr = new StreamReader(path);
				var jsonText = sr.ReadToEnd();
				sr.Close();
				return Courseware.CreateFromJson(jsonText);
			}

			return null;
		}

		public static void SaveCourseware(Courseware courseware)
		{
			if (courseware.Title.IsNullOrEmpty())
			{
				return;
			}
			var path = Path.Combine(ResourceDirectory, courseware.Root);

			if(!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			path = Path.Combine(path, CoursewareFileName);

			StreamWriter sw = File.CreateText(path);
			string data = courseware.ToJson();
			sw.Write(data);
			sw.Flush();
			sw.Close();
		}

		public static void Close()
		{
			Save();
		}
	}
}
