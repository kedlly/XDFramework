

using System;

namespace Framework.Library.ObjectPool
{
	public interface IPoolable
	{
		void OnAllocate();
		void OnRecycle();
	}

	public interface IMemoryPool
	{
		Type ObjectType { get; }
		int UnusedObjectCount { get; }
		int TotalObjectCount { get; }
		void ReleaseUnusedObjects();
		
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
