

namespace Framework.Library.ObjectPool
{
	using Framework.Utils.Extensions;
	using Policies;
	using System;
	public class SharedObjectPool<T, U> : IObjectPool<T> 
		where T : class
		where U : SharedBufferPolicy<T>
	{
		protected SharedBufferPolicy<T> implementPool { get; private set; }

		public Type ObjectType { get { return ((IObjectPool<T>)implementPool).ObjectType; } }

		public int UnusedObjectCount { get { return ((IObjectPool<T>)implementPool).UnusedObjectCount; } }

		public int TotalObjectCount { get { return ((IObjectPool<T>)implementPool).TotalObjectCount; } }

		public int ReserveDeepth
		{
			get { return ((IObjectPool<T>)implementPool).ReserveDeepth; }

			set { ((IObjectPool<T>)implementPool).ReserveDeepth = value; }
		}

		public int MaxDeepth { get { return ((IObjectPool<T>)implementPool).MaxDeepth; } set { ((IObjectPool<T>)implementPool).MaxDeepth = value; } }

		public IObjectFactory<T> ObjectFactory
		{
			get
			{
				return ((IObjectPool<T>)implementPool).ObjectFactory;
			}
		}

		public SharedObjectPool(IObjectFactory<T> factory = null)
		{
			implementPool = (SharedBufferPolicy<T>)Activator.CreateInstance(typeof(U), factory);
		}

		public virtual T Allocate(T template = null)
		{
			T obj = ((IObjectPool<T>)implementPool).Allocate(template);
			if (obj.ImplementsInterface<IPoolable>())
			{
				((IPoolable)obj).OnAllocated();
			}
			return obj;
		}

		public virtual bool Recycle(T obj)
		{
			bool succeed = ((IObjectPool<T>)implementPool).Recycle(obj);
			if (succeed && obj.ImplementsInterface<IPoolable>())
			{
				((IPoolable)obj).OnRecycled();
			}
			return succeed;
		}

		public virtual void ReleaseUnusedObjects()
		{
			((IObjectPool<T>)implementPool).ReleaseUnusedObjects();
		}

		public void ReleaseAllObjects()
		{
			((IObjectPool<T>)implementPool).ReleaseAllObjects();
		}
	}
}
