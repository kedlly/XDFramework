using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;


namespace Framework.Library.ObjectPool
{
	public class ObjectCache
	{
		private readonly Dictionary<Type, IMemoryPool> _objectPools = new Dictionary<Type, IMemoryPool>();


		private static ObjectCache _globalCache = new ObjectCache();
		public static ObjectCache GlobalCache { get { return _globalCache; } }

		public T Allocate<T>() where T : class
		{
			return this.GetOrCreateObjectPool<T>().Allocate();
		}

		public T Allocate<T, TFactory>()
			where T : class
			where TFactory : IObjectFactory<T>, new()
		{
			return this.GetOrCreateObjectPool<T, TFactory>().Allocate();
		}

		public void Recycle<T>(T obj) where T : class
		{
			this.GetOrCreateObjectPool<T>().Recycle(obj); // the return value is always true (or succeed) that because use stack memory by default.
		}

		public void RegisterCustomObjectPool<T>(IObjectPool<T> objectPool) where T : class
		{
			if (objectPool != null)
			{
				this._objectPools.Add(typeof(T), objectPool);
			}
		}

		public void RemoveExistObjectPool<T>() where T : class
		{
			if (!ContainsPool<T>())
			{
				return;
			}
			var targetPool = this._objectPools[typeof(T)] as IObjectPool<T>;
			this._objectPools.Remove(typeof(T));
			targetPool.ReleaseUnusedObjects();
			if (targetPool.TotalObjectCount != 0)
			{
				throw new Exception("ObjectCache Error: MemoryPool object already has some activated object that has Type : %s, count: %d"
					.FormatEx(typeof(T).ToString(), targetPool.TotalObjectCount));
			}
		}

		public void ReleaseUnusedObjects()
		{
			foreach (var imp in _objectPools.Values)
			{
				if (imp != null && imp.UnusedObjectCount > 0)
				{
					imp.ReleaseUnusedObjects();
				}
			}
		}

		public void Clean()
		{
			foreach (var imp in _objectPools.Values)
			{
				if (imp != null && imp.TotalObjectCount > 0)
				{
					imp.ReleaseAllObjects();
				}
			}
			this._objectPools.Clear();
		}

		private IObjectPool<T> CreateObjectPool<T>(IObjectFactory<T> factory = null) where T : class
		{
			return factory.CreatePool<T, Policies.StackMemory.PoolBufferPolicy<T>>();
		}

		public bool ContainsPool<T>()
		{
			return this._objectPools.ContainsKey(typeof(T));
		}

		private IObjectPool<T> GetOrCreateObjectPool<T>() where T : class
		{
			Type key = typeof(T);
			IMemoryPool objPool = null;
			if (!this._objectPools.TryGetValue(key, out objPool))
			{
				var newPool = CreateObjectPool<T>(null);
				RegisterCustomObjectPool(newPool);
				objPool = newPool;
			}
			return (IObjectPool<T>)objPool;
		}

		private IObjectPool<T> GetOrCreateObjectPool<T, TFactory>() 
			where T : class
			where TFactory : IObjectFactory<T>, new()
		{
			Type key = typeof(T);
			IMemoryPool objPool = null;
			if (!this._objectPools.TryGetValue(key, out objPool))
			{
				var newPool = CreateObjectPool(new TFactory());
				RegisterCustomObjectPool(newPool);
				objPool = newPool;
			}
			return (IObjectPool<T>)objPool;
		}
	}
}
