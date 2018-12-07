using UnityEngine;
using Framework.Library.Singleton;
using Framework.Library.ObjectPool;
using System.Collections.Generic;
using System.Collections;
using Framework.Utils.Extensions;
using Framework.Library.ObjectPool.Policies;

namespace Framework.Core
{
	public sealed class MemoryPoolManager : IMemoryPoolManager
	{
		private Dictionary<string, IMemoryPool> poolsInGame = new Dictionary<string, IMemoryPool>();

		public IObjectPool<T> CreatePool<T>(string name, IObjectFactory<T> factory = null) where T : class
		{
			var pool = GetObjectPool<T>(name);
			if(pool == null)
			{
				pool = factory.CreatePool();
				poolsInGame.Add(name, pool);
			}
			return pool;
		}

		public IObjectPool<TObject> CreatePool<TObject, TFactory, TBufferPolicy>(string name)
			where TFactory : IObjectFactory<TObject>, new()
			where TBufferPolicy : BufferPolicy<TObject>
			where TObject : class
		{
			var pool = GetObjectPool<TObject>(name);
			if(pool == null)
			{
				pool = new TFactory().CreatePool<TObject, TBufferPolicy>();
				poolsInGame.Add(name, pool);
			}
			return pool;
		}

		public IObjectPool<TObject> CreatePool<TObject, TBufferPolicy>(string name)
			where TBufferPolicy : BufferPolicy<TObject>
			where TObject : class
		{
			var pool = GetObjectPool<TObject>(name);
			if (pool == null)
			{
				IObjectFactory<TObject> itf = null;
				pool = itf.CreatePool<TObject, TBufferPolicy>();
				poolsInGame.Add(name, pool);
			}
			return pool;
		}

		public IObjectPool<TObject> CreateSharedPool<TObject>(string name, IObjectFactory<TObject> factory = null) where TObject : class
		{
			var pool = GetObjectPool<TObject>(name);
			if (pool == null)
			{
				pool = factory.CreateSharedPool();
				poolsInGame.Add(name, pool);
			}
			return pool;
		}

		public IObjectPool<TObject> CreateSharedPool<TObject, TFactory>(string name) 
			where TObject : class
			where TFactory : IObjectFactory<TObject>, new()
		{
			var pool = GetObjectPool<TObject>(name);
			if (pool == null)
			{
				pool = new TFactory().CreateSharedPool();
				poolsInGame.Add(name, pool);
			}
			return pool;
		}

		public void Dispose()
		{
			ReleaseAllUnusedPoolableObjects();
		}

		public IObjectPool<T> GetObjectPool<T>(string name) where T : class
		{
			return poolsInGame.ContainsKey(name) &&
					poolsInGame[name] as IObjectPool<T> != null &&
					(poolsInGame[name] as IObjectPool<T>).ObjectType == typeof(T) ?
					poolsInGame[name] as IObjectPool<T> :
					null;
		}

		public void ReleaseAllUnusedPoolableObjects()
		{
			poolsInGame.ForEach	( (k, v) => { v.ReleaseUnusedObjects(); } );
		}

		public void ReleasePool<T>(string name) where T : class
		{
			var pool = GetObjectPool<T>(name);
			if(pool != null)
			{
				if(pool.TotalObjectCount != 0)
				{
					pool.ReleaseUnusedObjects();
				}
				if (pool.TotalObjectCount == 0)
				{
					poolsInGame.Remove(name);
				}
			}
		}
	}
}