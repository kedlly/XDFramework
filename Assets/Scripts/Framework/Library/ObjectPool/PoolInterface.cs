

using System;

namespace Framework.Library.ObjectPool
{
	public interface IPoolable
	{
		void OnAllocated();
		void OnRecycled();
	}

	public interface IMemoryPool
	{
		Type ObjectType { get; }
		int UnusedObjectCount { get; }
		int TotalObjectCount { get; }
		void ReleaseUnusedObjects();
		// use this function carefully please. maybe some activated object also released, then the TotalObjectCount is 0, pool is empty now
		void ReleaseAllObjects();
	}

	public interface IObjectPool<T> : IMemoryPool
	{
		T Allocate();

		bool Recycle(T obj);
	}

	public interface IObjectFactory<T>
	{
		T Create();
		void Release(T obj);
	}

}
