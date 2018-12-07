using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Xml.XPath;

namespace XDDQFrameWork.Editor.ProjectBuilder
{

	public static class DataCollection
    {

		public const string ProjectBuildSettingsPathDefault = "./ProjectBuildSettings/Settings/";
		public const string ProjectBuildScriptsPathDefault = "./ProjectBuildSettings/Scripts/";

		public const string ProjectBuildPackagePath = "./ProjectBuild/Settings/";
		public const string ProjectBuildPackageTemplatePath = "./ProjectBuild/Templates/";
		public const string PackageAndroid = "Android";
		public const string PackageIOS = "iOS";
		public const string SettingFileName = "target.xml";
		public const string ShellFileName = "Package.sh";

		private const bool DEBUG_FLAG = false;
		private const string XML_TAG_ROOT = "BuildSettings";
		private const string XML_TAG_SCENES = "Scenes";
		private const string XML_TAG_SCENE = "Scene";
		private const string XML_TAG_PLAYERSETTINGS = "PlayerSettings";
		private const string XML_TAG_PS_GROUP_ANDROID = "PlayerSettings.Android";
		private const string XML_TAG_PS_GROUP_IOS = "PlayerSettings.iOS";
		private const string XML_TAG_GROUP_ANDROID = "Android";
		private const string XML_TAG_GROUP_IOS = "iOS";
		private const string XML_TAG_PS_EXT = "PSEXInfo";
		private const string _EXP_TYPE_PREFIX_ = "OtherType";
		private const char _SEP_ = '|';


		//private static string ERROR_MESSAGE_1 = "选项中包含 " + _SEP_ + " 字符, 目前无法进行导出, 如有必要,联系kedllyzhao";

		static DataCollection()
		{
			DirectoryInfo info = new DirectoryInfo(ProjectBuildPackageTemplatePath);
			info.Attributes = FileAttributes.ReadOnly;
		}

		public static string GetFilePathDefault(string fileName)
		{
			var di = Directory.CreateDirectory(ProjectBuildSettingsPathDefault);
			return di.FullName + "/" + fileName;
		}

		public static string GetPackageTargetXML(string packageName)
		{
			var di = new DirectoryInfo(Path.Combine(ProjectBuildPackagePath, packageName));
			if (!di.Exists)
			{
				throw new Exception("不存在的Build Package路径");
			}
			return Path.Combine(di.FullName, SettingFileName);
		}

		private static string ConvertPSValueToString(object obj)
		{
			string value = "null";
			if ( obj != null )
			{
				var valueType = obj.GetType();
				if ( valueType == typeof(string) || valueType == typeof(bool) || valueType == typeof(int) )
				{
					value = obj.ToString();
				}
				else if ( valueType.IsEnum )
				{
					value = valueType.FullName + _SEP_ + obj.ToString();
				}
				else
				{
					value = _EXP_TYPE_PREFIX_ + _SEP_ + valueType.FullName + _SEP_ + obj.ToString();
				}
			}
			return value;
		}

		private static void ExportSettings(Type type, XmlElement element, Func<string, bool> filterIgone = null)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
			foreach ( var prop in properties )
			{
				string name = prop.Name;
				if ( filterIgone != null && filterIgone(name) )
				{
					continue;
				}
				object ov = prop.GetValue(null, null);
				//string value = ConvertPSValueToString(ov);
				var newEle = element.OwnerDocument.CreateElement(name);
				element.AppendChild(newEle);
				try
				{
					if ( ov != null )
					{
						newEle.SetAttribute("Type", ov.GetType().FullName);
						newEle.InnerText = ov.ToString();
					}
					else
					{
						newEle.SetAttribute("Type", "null");
						//newEle.InnerText = null;
					}

				}
				catch ( System.Exception ex )
				{
					UnityEngine.Debug.Log(ex.ToString());
				}

			}
		}

		private static bool PlayerSettingFilter(string name)
		{
			return name.StartsWith("xbox") ||
					name.StartsWith("virtualReality") ||
					name.StartsWith("cloud") ||
					name.EndsWith("bundleIdentifier", StringComparison.OrdinalIgnoreCase);

		}

		private const string AdditionalIl2CppArgs = "AdditionalIl2CppArgs";
		private const string ApiCompatibilityLevel = "ApiCompatibilityLevel";
		private const string ApplicationIdentifier = "ApplicationIdentifier";
		private const string Architecture = "Architecture";
		private const string IncrementalIl2CppBuild = "IncrementalIl2CppBuild";
		private const string GraphicsAPIs = "GraphicsAPIs";
		private const string ScriptingBackend = "ScriptingBackend";
		private const string ScriptingDefineSymbolsForGroup = "ScriptingDefineSymbolsForGroup";
		private const string UseDefaultGraphicsAPIs = "UseDefaultGraphicsAPIs";
		private const string GraphicsAPIs4 = "GraphicsAPIs";
		private const string GraphicsAPIs5 = "GraphicsAPIs";

		private static string _InnerTextOf(object obj)
		{
			string ret = "";
			if ( obj != null )
			{
				var objType = obj.GetType();
				if ( objType.IsArray )
				{
					var values = obj as Array;
					if ( values != null )
					{
						string[] data = new string[values.Length];
						for ( int i = 0; i < values.Length; ++i )
						{
							data[i] = values.GetValue(i).ToString();
						}
						ret = string.Join("|", data);
					}
					else
					{
						throw new Exception("Error Type");
					}
				}
				else
				{
					ret = obj.ToString();
				}
			}
			return ret;
		}

		private static void ExportPSEXInfo(XmlElement exInfo)
		{
			exInfo.SetAttribute(AdditionalIl2CppArgs, ConvertPSValueToString(PlayerSettings.GetAdditionalIl2CppArgs()));

			string[] funcNames = { ApiCompatibilityLevel
									, ApplicationIdentifier
									, Architecture
									, IncrementalIl2CppBuild
									, GraphicsAPIs
									, ScriptingBackend
									, ScriptingDefineSymbolsForGroup
									, UseDefaultGraphicsAPIs
								};
			foreach ( string fn in funcNames )
			{
				Type theType = typeof(PlayerSettings);
				MethodInfo mi = theType.GetMethod("Get" + fn, BindingFlags.Public | BindingFlags.Static);
				if ( mi != null )
				{
					ParameterInfo[] pis = mi.GetParameters();
					object androidData = null;
					object iOSData = null;
					if ( pis[0].ParameterType == typeof(BuildTargetGroup) )
					{
						androidData = mi.Invoke(null, new object[] { BuildTargetGroup.Android });
						iOSData = mi.Invoke(null, new object[] { BuildTargetGroup.iOS });
					}
					else if ( pis[0].ParameterType == typeof(BuildTarget) )
					{
						androidData = mi.Invoke(null, new object[] { BuildTarget.Android });
						iOSData = mi.Invoke(null, new object[] { BuildTarget.iOS });
					}
					var newEle = exInfo.OwnerDocument.CreateElement(fn);
					exInfo.AppendChild(newEle);
					var newAndroidEle = newEle.OwnerDocument.CreateElement(XML_TAG_GROUP_ANDROID);
					var newIOSEle = newEle.OwnerDocument.CreateElement(XML_TAG_GROUP_IOS);
					newEle.AppendChild(newAndroidEle);
					newEle.AppendChild(newIOSEle);
					newEle.SetAttribute("Type", mi.ReturnType.FullName);
					if ( androidData != null )
					{
						newAndroidEle.InnerText = _InnerTextOf(androidData);
					}
					if ( iOSData != null )
					{
						newIOSEle.InnerText = _InnerTextOf(iOSData);
					}
				}


			}

		}

		public static void ExportBuildSettingToXML(string filePath)
		{
			//EditorUserBuildSettings.activeBuildTarget
			var scenes = EditorBuildSettings.scenes;
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement(XML_TAG_ROOT);
			
			doc.AppendChild(root);

			XmlDeclaration xmldecl;
			xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
			xmldecl.Encoding = "UTF-8";
			xmldecl.Standalone = "yes";

			doc.InsertBefore(xmldecl, root);

			XmlElement playerSettings = doc.CreateElement(XML_TAG_PLAYERSETTINGS);
			root.AppendChild(playerSettings);
			ExportSettings(typeof(PlayerSettings), playerSettings, PlayerSettingFilter);
			var androidElement = doc.CreateElement(XML_TAG_PS_GROUP_ANDROID);
			ExportSettings(typeof(PlayerSettings.Android), androidElement);
			root.AppendChild(androidElement);
			var iOSElement = doc.CreateElement(XML_TAG_PS_GROUP_IOS);
			ExportSettings(typeof(PlayerSettings.iOS), iOSElement);
			root.AppendChild(iOSElement);

			// other info

			var exInfo = doc.CreateElement(XML_TAG_PS_EXT);
			ExportPSEXInfo(exInfo);
			root.AppendChild(exInfo);

			if ( !filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) )
			{
				filePath += ".xml";
			}
			doc.Save(filePath);
		}

		public static void ReloadBuildSettings(BuildTarget buildTarget)
		{
			if ( buildTarget == BuildTarget.Android )
			{
				EditorUserBuildSettings.allowDebugging = true;
				EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
				EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
				EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
			}
			else if ( buildTarget == BuildTarget.iOS )
			{
				EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void ImportBuildSettingFromXML(string filepath)
		{
			if ( !filepath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) )
			{
				filepath += ".xml";
			}
			XmlDocument doc = new XmlDocument();
			doc.Load(filepath);
			var root = doc.GetElementsByTagName(XML_TAG_ROOT)[0];
			if ( root != null )
			{
				foreach ( XmlNode node in root.ChildNodes )
				{
					switch ( node.Name )
					{
					case XML_TAG_PLAYERSETTINGS:
						{
							foreach ( XmlNode propNode in node )
							{
								object obj = GetPropertyFromXMLNode(propNode);
								ImportPorperty(typeof(PlayerSettings), propNode.Name, obj);
							}
						}
						break;
					case XML_TAG_PS_GROUP_ANDROID:
						{
							foreach ( XmlNode propNode in node )
							{
								object obj = GetPropertyFromXMLNode(propNode);
								ImportPorperty(typeof(PlayerSettings.Android), propNode.Name, obj);
							}
						}
						break;
					case XML_TAG_PS_GROUP_IOS:
						{
							foreach ( XmlNode propNode in node )
							{
								object obj = GetPropertyFromXMLNode(propNode);
								ImportPorperty(typeof(PlayerSettings.iOS), propNode.Name, obj);
							}
						}
						break;
					case XML_TAG_PS_EXT:
						{
							foreach ( XmlAttribute attr in node.Attributes )
							{
								var _name = attr.Name;
								var mi = typeof(PlayerSettings).GetMethod("Set" + _name, BindingFlags.Static | BindingFlags.Public);
								if ( mi != null )
								{
									mi.Invoke(null, new object[] { attr.Value });
								}
							}
							foreach ( XmlNode propNode in node )
							{
								var propName = propNode.Name;
								var TypeText = propNode.Attributes[0].Value;
								var mi = typeof(PlayerSettings).GetMethod("Set" + propName, BindingFlags.Static | BindingFlags.Public);
								if ( mi != null )
								{
									var pis = mi.GetParameters();
									if ( pis[0].ParameterType == typeof(BuildTargetGroup) )
									{
										foreach ( XmlNode subNode in propNode )
										{
											var _param = ParserValue(propNode.Attributes[0].Value, subNode.InnerText);
											if ( subNode.Name == XML_TAG_GROUP_ANDROID )
											{
												mi.Invoke(null, new object[] { BuildTargetGroup.Android, _param });
											}
											else if ( subNode.Name == XML_TAG_GROUP_IOS )
											{
												mi.Invoke(null, new object[] { BuildTargetGroup.iOS, _param });
											}
										}
									}
									else if ( pis[0].ParameterType == typeof(BuildTarget) )
									{
										foreach ( XmlNode subNode in propNode )
										{
											var _param = ParserValue(propNode.Attributes[0].Value, subNode.InnerText);
											if ( subNode.Name == XML_TAG_GROUP_ANDROID )
											{
												mi.Invoke(null, new object[] { BuildTarget.Android, _param });
											}
											else if ( subNode.Name == XML_TAG_GROUP_IOS )
											{
												mi.Invoke(null, new object[] { BuildTarget.iOS, _param });
											}
										}
									}

								}
								//var valueText = node.
								//ParserValue(TypeAttr.Value, node.InnerText);

							}
						}
						break;
					}
				}
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void ExportProjectSettings(string pathFolder)
		{
			var d = Directory.CreateDirectory(pathFolder);
			DirectoryCopy(ProjectBuildPackageTemplatePath, d.FullName, true, false);
			ExportBuildSettingToXML(Path.Combine(pathFolder, SettingFileName));
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			// If the source directory does not exist, throw an exception.
			if ( !dir.Exists )
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory does not exist, create it.
			if ( !Directory.Exists(destDirName) )
			{
				Directory.CreateDirectory(destDirName);
			}


			// Get the file contents of the directory to copy.
			FileInfo[] files = dir.GetFiles();

			foreach ( FileInfo file in files )
			{
				// Create the path to the new copy of the file.
				string temppath = Path.Combine(destDirName, file.Name);

				// Copy the file.
				try
				{
					file.CopyTo(temppath, overwrite);
				}
				catch (IOException )
				{
					
				}
				
			}

			// If copySubDirs is true, copy the subdirectories.
			if ( copySubDirs )
			{

				foreach ( DirectoryInfo subdir in dirs )
				{
					// Create the subdirectory.
					string temppath = Path.Combine(destDirName, subdir.Name);

					// Copy the subdirectories.
					DirectoryCopy(subdir.FullName, temppath, copySubDirs, overwrite);
				}
			}
		}

		private static Type GetTypeFrom(string typeName)
		{
			Assembly[] otherAssemblies = {
					typeof(BuildPipeline).Assembly
					, typeof(RenderingPath).Assembly
				};
			Type TargetType = Type.GetType(typeName);
			if ( TargetType == null)
			{
				for ( int i = 0; i < otherAssemblies.Length; ++i )
				{
					TargetType = otherAssemblies[i].GetType(typeName);
					if ( TargetType != null)
					{
						break;
					}
				}
			}
			return TargetType;
		}
		private static object ParserValue(string typeName, string propValue)
		{
			var propType = GetTypeFrom(typeName);
			object obj = null;
			if ( propType != null )
			{
				if ( propType == typeof(int) )
				{
					int _value = 0;
					if ( int.TryParse(propValue, out _value) )
					{

					}
					obj = _value;
				}
				else if ( propType == typeof(string) )
				{
					obj = propValue;
				}
				else if ( propType == typeof(bool) )
				{
					bool _value = false;
					if ( bool.TryParse(propValue, out _value) )
					{

					}
					obj = _value;
				}
				else if ( propType == typeof(Guid) )
				{
					obj = new GUID(propValue);
				}
				else if ( propType.IsEnum )
				{
					obj = Enum.Parse(propType, propValue);
				}
				else
				{
					if ( propType.IsArray )
					{
						Type eleType = propType.GetElementType();
						var valueArray = propValue.Split(_SEP_);
						Array array = Array.CreateInstance(eleType, valueArray.Length);
						for ( int i = 0; i < valueArray.Length; ++i )
						{
							object _value = ParserValue(eleType.FullName, valueArray[i]);
							array.SetValue(_value, i);
						}
						obj = array;
					}
				}
			}
			else
			{

			}
			return obj;
		}

		private static object GetPropertyFromXMLNode(XmlNode node)
		{
			var propName = node.Name;
			var TypeAttr = node.Attributes[0];
			return ParserValue(TypeAttr.Value, node.InnerText);
		}

		private static void ImportPorperty(Type targetType, string propertyName, object value, BindingFlags propertyFlag = BindingFlags.Public | BindingFlags.Static, object targetObject = null)
		{
			var pi = targetType.GetProperty(propertyName, propertyFlag);
			if ( pi != null )
			{
				if ( pi.CanWrite )
				{
					pi.SetValue(targetObject, value, null);
				}
			}
		}

	}


}
