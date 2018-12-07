using System;
using System.Reflection;

namespace Framework.Library.Singleton
{

	public interface ISingleton
	{
		void Initialize();
		void Dispose();
		void CallbackOnReborn();
	}

	internal static class Singleton<T> where T : class, ISingleton
	{
		static volatile T instance;
		private static readonly object _lock = new object();
		static bool instance_disposed = false;

		static Singleton()
		{

		}

		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					lock(_lock)
					{
						if (instance == null)
						{
							ConstructorInfo constructor = null;
							try
							{
								constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
							}
							catch (Exception ex)
							{
								throw (ex);
							}

							if ( constructor == null || constructor.IsAssembly)
							{
								throw new Exception(string.Format("A private or protected constructor with no params is missing for '{0}'.", typeof(T).Name));
							}

							instance = (T)constructor.Invoke(null);
							instance.Initialize();
							if(instance_disposed)
							{
								instance.CallbackOnReborn();
							}
						}
					}
				}
				return instance;
			}
		}

		public static void DisposeSingleton()
		{
			if (instance != null)
			{
				lock(_lock)
				{
					if(instance != null)
					{
						instance = null;
						instance_disposed = true;
					}
				}
			}
		}
	}

	public abstract class ToSingleton<T> : ISingleton where T : ToSingleton<T>
	{
		public static T Instance
		{
			get { return Singleton<T>.Instance; }
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
			if (OnEventDispose != null)
			{
				OnEventDispose();
			}
		}

		public event Action OnEventDispose;

		protected abstract void OnSingletonInit();

		protected virtual void OnDispose()
		{
			Singleton<T>.DisposeSingleton();
		}

		protected virtual void OnSingletonReborn()
		{

		}

		
	}
}
