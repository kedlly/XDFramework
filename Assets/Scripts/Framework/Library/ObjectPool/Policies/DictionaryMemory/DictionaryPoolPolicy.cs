using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Library.ObjectPool.Policies.DictionaryMemory
{
	public class DictionaryPolicy<T> : BufferPolicy<T> where T : class
	{
		private const int capacity = 8;

		Dictionary<T, bool> dictCache = new Dictionary<T, bool>(capacity); // T, isRecycled

		protected override int UnusedObjectCount {  get { return dictCache.Count(it => !it.Value); } }

		protected override int TotalObjectCount { get { return dictCache.Count; } }

		public DictionaryPolicy(IObjectFactory<T> factory = null) : base(factory)	{ }

		protected override T OnAllocate()
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

		protected override bool OnRecycle(T obj)
		{
			bool result = false;
			if(dictCache.ContainsKey(obj) && !dictCache[obj])
			{
				dictCache[obj] = true;
				result = true;
			}
			return result;
		}

		List<T> tmpRelease = new List<T>();
		Dictionary<T, bool> tmpStorage = new Dictionary<T, bool>();

		protected override void OnReleaseUnusedObjects()
		{
			var it = dictCache.GetEnumerator();
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
			var tmpRef = dictCache;
			dictCache = tmpStorage;
			tmpStorage = tmpRef;
			tmpRelease.ForEach(value => ObjectFactory.Release(value));
			tmpRelease.Clear();
			tmpStorage.Clear();
		}

		protected override void OnReleaseAllObjects()
		{
			foreach (var item in dictCache)
			{
				ObjectFactory.Release(item.Key);
			}
			dictCache.Clear();
		}
	}

	public class PoolBufferPolicy<T> : DictionaryPolicy<T> where T : class
	{
		public PoolBufferPolicy(IObjectFactory<T> factory = null) : base(factory) { }
	}
}
