using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Library.ObjectPool.Policy
{
	public class DictPoolBuffer<T> : IObjectPool<T> where T : class
	{
		private const int capacity = 8;

		Dictionary<T, bool> dictCache = new Dictionary<T, bool>(capacity); // T, isRecycled

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

		int IMemoryPool.TotalObjectCount { get { return dictCache.Count; } }

		public DictPoolBuffer(IObjectFactory<T> factory)
		{
			ObjectFactory = factory;
		}

		private int GetUnusedObjectCount()
		{
			return dictCache.Count(it => !it.Value);
		}


		public virtual T Allocate()
		{
			T obj = null;
			var it = dictCache.GetEnumerator();
			while(it.MoveNext())
			{
				if(it.Current.Value)
				{
					obj = it.Current.Key;
					break;
				}
			}
			if(obj == null)
			{
				obj = ObjectFactory.Create();
			}
			dictCache[obj] = false;
			return obj;
		}

		public virtual bool Recycle(T obj)
		{
			bool result = false;
			if(dictCache.ContainsKey(obj) && !dictCache[obj])
			{
				dictCache[obj] = true;
				result = true;
			}
			return result;
		}

		void IMemoryPool.ReleaseUnusedObjects()
		{
			var it = dictCache.GetEnumerator();
			List<T> tmpRelease = new List<T>();
			Dictionary<T, bool> tmpStorage = new Dictionary<T, bool>();
			while(it.MoveNext())
			{
				if(it.Current.Value)
				{
					tmpRelease.Add(it.Current.Key);
				}
				else
				{
					tmpStorage[it.Current.Key] = false;
				}
			}
			dictCache = tmpStorage;
			tmpRelease.ForEach(value => ObjectFactory.Release(value));
		}
	}
}
