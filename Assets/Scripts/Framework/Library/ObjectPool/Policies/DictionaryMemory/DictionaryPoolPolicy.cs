using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Library.ObjectPool.Policies.DictionaryMemory
{
	
	public abstract class ADictionaryPolicy<T> : BufferPolicy<T> where T : class
	{
		private const int capacity = 8;
		protected override int UnusedObjectCount { get { return releaseCache.Count; } }

		protected override int TotalObjectCount { get { return Cache.Count + releaseCache.Count; } }

		private Dictionary<T, int> Cache = new Dictionary<T, int>(capacity);
		private HashSet<T> releaseCache = new HashSet<T>();

		public ADictionaryPolicy(IObjectFactory<T> factory = null) : base(factory) { }

		protected T OnAllocateByRefcount(T template)
		{
			T newObj = PickInCache(template);
			if (newObj == null)
			{
				newObj = ReuseOrCreateItem(template);
				Cache[newObj] = 0;
			}
			Cache[newObj]++;
			return newObj;
		}

		protected T OnAllocateNormal(T template)
		{
			T newObj = ReuseOrCreateItem(template);
			Cache[newObj] = 1;
			return newObj;
		}

		private T ReuseOrCreateItem(T template)
		{
			T newObj = PickInReleaseCache(template);
			if (newObj != null)
			{
				releaseCache.Remove(newObj);
				if (newObj != template)
				{
					ObjectFactory.Copy(newObj, template);
				}
			}
			else
			{
				newObj = ObjectFactory.Create();
				ObjectFactory.Copy(newObj, template);
			}
			return newObj;
		}

		private T PickInReleaseCache(T template)
		{
			if (releaseCache.Count > 0)
			{
				if (template != null && releaseCache.Contains(template))
				{
					return template;
				}
				return releaseCache.First();
			}
			return null;
		}

		private T PickInCache(T template)
		{
			return template != null && Cache.ContainsKey(template) ? template : null;
		}

		protected override bool OnRecycle(T obj)
		{
			if (Cache.ContainsKey(obj))
			{
				Cache[obj]--;
				if (Cache[obj] == 0)
				{
					Cache.Remove(obj);
					releaseCache.Add(obj);
				}
				return true;
			}
			return false;
		}

		protected override void OnReleaseAllObjects()
		{
			Cache.ForEach(it => ObjectFactory.Release(it.Key));
			Cache.Clear();
			releaseCache.ForEach(it => ObjectFactory.Release(it));
			releaseCache.Clear();
		}

		protected override void OnReleaseUnusedObjects(int count = -1)
		{
			if (count == -1 || count > releaseCache.Count)
			{
				count = releaseCache.Count;
			}
			if (count > 0)
			{
				releaseCache.Take(count).ToArray().ForEach(it =>
				{
					releaseCache.Remove(it);
					ObjectFactory.Release(it);
				});
			}
		}
	}


	public class DictionaryPolicy<T> : ADictionaryPolicy<T> where T : class
	{
		public DictionaryPolicy(IObjectFactory<T> factory) : base(factory) { }

		protected override T OnAllocate(T objectTemplate = null)
		{
			return OnAllocateNormal(objectTemplate);
		}
	}

	public class PoolBufferPolicy<T> : DictionaryPolicy<T> where T : class
	{
		public PoolBufferPolicy(IObjectFactory<T> factory = null) : base(factory) { }
	}
}
