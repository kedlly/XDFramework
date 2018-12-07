
using System;

namespace Framework.Library.ObjectPool.Policies
{
	public abstract class BufferPolicy<T> : IObjectPool<T> where T : class
	{
		Type IMemoryPool.ObjectType { get { return typeof(T); } }

		public IObjectFactory<T> ObjectFactory { get; private set; }

		public BufferPolicy(IObjectFactory<T> objectFactory = null)
		{
			ObjectFactory = objectFactory ?? DefaultObjectFactory.theFactory;
			reserveDeepth = maxDeepth = -1;
		}

		private sealed class DefaultObjectFactory : IObjectFactory<T>
		{
			public static readonly DefaultObjectFactory theFactory;
			static DefaultObjectFactory()
			{
				theFactory = new DefaultObjectFactory();
			}

			public void Copy(T target, T template)
			{
				
			}

			T IObjectFactory<T>.Create()
			{
				return Activator.CreateInstance<T>();
			}

			void IObjectFactory<T>.Release(T obj)
			{
				
			}
		}

		protected abstract T OnAllocate(T objectTemplate = null);
		protected abstract bool OnRecycle(T obj);
		protected abstract void OnReleaseUnusedObjects(int count = -1); // -1 is all of unused objects;
		protected abstract void OnReleaseAllObjects();

		private object _locker = new object();

		T IObjectPool<T>.Allocate(T objectTemplate)
		{
			lock(_locker)
			{
				if (maxDeepth != -1 && (this.TotalObjectCount >= maxDeepth && this.UnusedObjectCount == 0))
				{
					throw new Exception("bad allocation, pool is full.");
				}
				return OnAllocate(objectTemplate);
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
				if (reserveDeepth != -1 && UnusedObjectCount > reserveDeepth)
				{
					OnReleaseUnusedObjects(UnusedObjectCount - reserveDeepth);
				}
				else
				{
					OnReleaseUnusedObjects(-1);
				}
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
		private int reserveDeepth { get; set; }
		private int maxDeepth { get; set; }

		int IMemoryPool.UnusedObjectCount { get { lock (_locker) return this.UnusedObjectCount; } }

		int IMemoryPool.TotalObjectCount { get { lock (_locker) return this.TotalObjectCount; } }

		int IMemoryPool.ReserveDeepth { get { lock (_locker) return this.reserveDeepth; } set { lock (_locker) { this.reserveDeepth = value; } } }

		int IMemoryPool.MaxDeepth { get { lock (_locker) return this.maxDeepth; } set { lock (_locker) { this.maxDeepth = value; } } }
	}

	public class SharedBufferPolicy<T> : DictionaryMemory.ADictionaryPolicy<T> where T : class
	{
		public SharedBufferPolicy(IObjectFactory<T> factory) : base(factory) { }

		protected override T OnAllocate(T objectTemplate = null)
		{
			return OnAllocateByRefcount(objectTemplate);
		}
	}
}
