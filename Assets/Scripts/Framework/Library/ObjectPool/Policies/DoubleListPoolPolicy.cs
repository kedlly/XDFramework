using System;
using System.Collections.Generic;

namespace Framework.Library.ObjectPool.Policy
{

	public class ListPoolBuffer<T> : IObjectPool<T> where T : class
	{
		private const int capacity = 8;

		private List<T> openSet = new List<T>(capacity);
		private List<T> closeSet = new List<T>(capacity);

		IObjectFactory<T> ObjectFactory = null;

		Type IMemoryPool.ObjectType
		{
			get
			{
				return typeof(T);
			}
		}

		int IMemoryPool.UnusedObjectCount
		{
			get
			{
				return GetUnusedObjectCount();
			}
		}

		int IMemoryPool.TotalObjectCount { get { return openSet.Count + closeSet.Count; } }

		private int GetUnusedObjectCount()
		{
			return openSet.Count;
		}

		public ListPoolBuffer(IObjectFactory<T> factory)
		{
			ObjectFactory = factory;
		}

		public virtual T Allocate()
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

		public virtual bool Recycle(T obj)
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

		void IMemoryPool.ReleaseUnusedObjects()
		{
			openSet.ForEach(item => ObjectFactory.Release(item));
			openSet.Clear();
		}
	}


}