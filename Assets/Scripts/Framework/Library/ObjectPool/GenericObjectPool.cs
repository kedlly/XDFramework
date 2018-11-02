﻿


namespace Framework.Library.ObjectPool
{
	using Policies;
	using System;

	public class ObjectPool<T, U> : IObjectPool<T> where T : class where U : BufferPolicy<T>
	{
		protected BufferPolicy<T> implementPool { get; private set; }

		public Type ObjectType { get { return ((IObjectPool<T>)implementPool).ObjectType; } }

		public int UnusedObjectCount { get { return ((IObjectPool<T>)implementPool).UnusedObjectCount; }	}

		public int TotalObjectCount { get { return ((IObjectPool<T>)implementPool).TotalObjectCount; }	}

		public ObjectPool(IObjectFactory<T> factory = null)
		{
			implementPool = (BufferPolicy<T> )Activator.CreateInstance(typeof(U), factory);
		}

		public virtual T Allocate()
		{
			return ((IObjectPool<T>)implementPool).Allocate();
		}

		public virtual bool Recycle(T obj)
		{
			return ((IObjectPool<T>)implementPool).Recycle(obj);
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

	public class PoolableObjectPool<T, U> : ObjectPool<T, U> where T : class, IPoolable where U : BufferPolicy<T>
	{
		public PoolableObjectPool(IObjectFactory<T> factory = null) : base (factory){  }

		public override T Allocate ()
		{
			T allocated = base.Allocate();
			if (allocated != null)
			{
				allocated.OnAllocated();
			}
			return allocated;
		}

		public override bool Recycle(T obj)
		{
			bool succeed = base.Recycle(obj);
			if (succeed)
			{
				obj.OnRecycled();
			}
			return succeed;
		}
	}
}
