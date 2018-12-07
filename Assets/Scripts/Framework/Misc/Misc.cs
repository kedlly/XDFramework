
using UnityEngine;


namespace Framework.Misc.Paths
{
	/*
		* Application.dataPath				此属性用于返回程序的数据文件所在文件夹的路径。例如在Editor中就是Assets了。
		* Application.streamingAssetsPath	此属性用于返回流数据的缓存目录，返回路径为相对路径，适合设置一些外部数据文件的路径。
		* Application.persistentDataPath	此属性用于返回一个持久化数据存储目录的路径，可以在此路径下存储一些持久化的数据文件。
		* Application.temporaryCachePath	此属性用于返回一个临时数据的缓存目录。
		* 
		* //in android platform
		* Application.dataPath				/data/app/xxx.xxx.xxx.apk
		* Application.streamingAssetsPath	jar:file:///data/app/xxx.xxx.xxx.apk/!/assets
		* Application.persistentDataPath	/data/data/xxx.xxx.xxx/files
		* Application.temporaryCachePath	/data/data/xxx.xxx.xxx/cache
		* 
		* // in iOS platform
		* Application.dataPath				Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/xxx.app/Data
		* Application.streamingAssetsPath	Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/xxx.app/Data/Raw
		* Application.persistentDataPath	Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Documents
		* Application.temporaryCachePath	Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Library/Caches
		* 
		*/
	internal static class DevicePath
	{
		public static string DataPath = Application.dataPath;
		public static string StreamingAssetsPath = Application.streamingAssetsPath;
		public static string TemporaryCachePath = Application.temporaryCachePath;
#if UNITY_EDITOR
		public static string PersistentDataPath = Application.dataPath + "/../PersistentPath";
#elif UNITY_STANDALONE
	public static string PersistentDataPath	= Application.dataPath + "/PersistentPath";
#else
	public static string PersistentDataPath	= Application.persistentDataPath;
#endif
	}
}


