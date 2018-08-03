
using UnityEngine;
using System.Reflection;
using Framework.Utils.Extensions;

namespace Framework.Library.Singleton
{
	[Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
	public class SingletonBehaviour<T> where T : MonoBehaviour, ISingleton
	{
		private struct InstanceTagData
		{
			public bool disposed;
			public bool initialized;
		}
		static volatile T instance;
		static InstanceTagData tagData = new InstanceTagData { disposed = false, initialized = false,  };

		public static void DisposeSingleton()
		{
			if(instance != null)
			{
				instance = null;
				tagData.disposed = true;
				tagData.initialized = false;
			}
		}

		public static T Instance
		{
			get
			{
				if(instance != null)
				{
					if(!tagData.initialized)
					{
						instance.Initialize();
						tagData.initialized = true;
					}
					return instance;
				}

				instance = UnityEngine.Object.FindObjectOfType<T>();

				if(instance != null)
				{
					if(!tagData.initialized)
					{
						instance.Initialize();
						tagData.initialized = true;
					}
					return instance;
				}

				instance = AutoFactory.Create<T>();

				if (!tagData.initialized)
				{
					instance.Initialize();
					tagData.initialized = true;
				}
				if (tagData.disposed)
				{
					instance.CallbackOnReborn();
				}

				return instance;
			}
		}

		private static T CreateComponentOnGameObject(string path, bool dontDestroyOnLoad)
		{
			var go = path.FindGameObject(null, true);
			if (dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(go.transform.root.gameObject);
			}
			return go.AddComponent<T>();
		}

	}

	[DontDestroyOnLoad]
	[PathInHierarchy("sssss")]
	[Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
	public abstract class ToSingletonBehavior<T> : MonoBehaviour, ISingleton where T : ToSingletonBehavior<T>
	{

		public static T Instance
		{
			get
			{
				return SingletonBehaviour<T>.Instance;
			}
		}

		void ISingleton.Initialize()
		{
			OnSingletonInit();
		}

		void ISingleton.CallbackOnReborn()
		{
			OnSingletonReborn();
		}

		public void Dispose()
		{
			OnDispose();
		}

		protected virtual void OnSingletonInit() {}

		protected virtual void OnDispose() { SingletonBehaviour<T>.DisposeSingleton(); }

		protected virtual void OnSingletonReborn() { }
	}

	
}