using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library.ObjectPool.Policies
{
	public abstract class BufferPolicy<T> : IObjectPool<T> where T : class
	{
		Type IMemoryPool.ObjectType { get { return typeof(T); } }

		protected IObjectFactory<T> ObjectFactory { get; private set; }

		public BufferPolicy(IObjectFactory<T> objectFactory = null)
		{
			ObjectFactory = objectFactory ?? DefaultObjectFactory.theFactory;
		}

		private sealed class DefaultObjectFactory : IObjectFactory<T>
		{
			public static readonly DefaultObjectFactory theFactory;
			static DefaultObjectFactory()
			{
				theFactory = new DefaultObjectFactory();
			}
			T IObjectFactory<T>.Create()
			{
				return Activator.CreateInstance<T>();
			}

			void IObjectFactory<T>.Release(T obj)
			{
				
			}
		}

		protected abstract T OnAllocate();
		protected abstract bool OnRecycle(T obj);
		protected abstract void OnReleaseUnusedObjects();
		protected abstract void OnReleaseAllObjects();

		private object _locker = new object();

		T IObjectPool<T>.Allocate()
		{
			lock(_locker)
			{
				return OnAllocate();
			}
		}

		bool IObjectPool<T>.Recycle(T obj)
		{
			lock (_locker)
			{
				return OnRecycle(obj);
			}
		}

		void IMemoryPool.ReleaseUnusedObjects()
		{
			lock (_locker)
			{
				OnReleaseUnusedObjects();
			}
		}

		void IMemoryPool.ReleaseAllObjects()
		{
			lock (_locker)
			{
				OnReleaseAllObjects();
			}
		}

		protected abstract int UnusedObjectCount { get; }
		protected abstract int TotalObjectCount { get; }

		int IMemoryPool.UnusedObjectCount { get { lock (_locker) return this.UnusedObjectCount; } }

		int IMemoryPool.TotalObjectCount { get { lock (_locker) return this.TotalObjectCount; } }
	}

	
}
