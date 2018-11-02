using System;
using System.Collections.Generic;

namespace Framework.Library.ObjectPool.Policies.DoubleListMemory
{
	public class DoubleListPolicy<T> : BufferPolicy<T> where T : class
	{
		private const int capacity = 8;

		private List<T> openSet = new List<T>(capacity);
		private List<T> closeSet = new List<T>(capacity);

		protected override int UnusedObjectCount { get	{ return openSet.Count;	} }

		protected override int TotalObjectCount { get { return openSet.Count + closeSet.Count; } }

		public DoubleListPolicy(IObjectFactory<T> factory = null) : base (factory) {}

		protected override T OnAllocate()
		{
			T obj = null;
			if(openSet.Count > 0)
			{
				obj = openSet[0];
				openSet.RemoveAt(0);
			}
			else
			{
				obj = ObjectFactory.Create();
			}
			closeSet.Add(obj);
			return obj;
		}

		protected override bool OnRecycle(T obj)
		{
			bool result = false;
			if(closeSet.Contains(obj))
			{
				closeSet.Remove(obj);
				openSet.Add(obj);
				result = true;
			}
			return result;
		}

		protected override void OnReleaseUnusedObjects()
		{
			openSet.ForEach(item => ObjectFactory.Release(item));
			openSet.Clear();
		}

		protected override void OnReleaseAllObjects()
		{
			openSet.ForEach(item => ObjectFactory.Release(item));
			openSet.Clear();
			closeSet.ForEach(item => ObjectFactory.Release(item));
			closeSet.Clear();
		}
	}

	public class PoolBufferPolicy<T> : DoubleListPolicy<T> where T : class
	{
		public PoolBufferPolicy(IObjectFactory<T> factory = null) : base(factory) { }
	}
}