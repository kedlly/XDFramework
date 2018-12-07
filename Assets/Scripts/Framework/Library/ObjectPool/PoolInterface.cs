

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
		int ReserveDeepth { get; set; }
		int MaxDeepth { get; set; }
		int UnusedObjectCount { get; }
		int TotalObjectCount { get; }
		void ReleaseUnusedObjects();
		// use this function carefully please. maybe some activated object also released, then the TotalObjectCount is 0, pool is empty now
		void ReleaseAllObjects();
	}

	public interface IObjectPool<T> : IMemoryPool where T : class
	{
		IObjectFactory<T> ObjectFactory { get; }
		T Allocate(T objectTemplate = null);
		bool Recycle(T obj);
	}

	public interface IObjectFactory<T> where T : class
	{
		T Create();
		void Copy(T target, T template);
		void Release(T obj);
	}

	public interface IReferenceTrack
	{
		void Retain(object owner);
		void Release(object owner);
		int  RetainCount { get; }
	}

}
