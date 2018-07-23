using UnityEngine;
using Framework.Library.Singleton;
using Framework.Library.ObjectPool;
using System.Collections.Generic;
using System.Collections;
using Framework.Utils.Extensions;

namespace Framework.Core
{
	[GameObjectPath("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class PoolsManager : ToSingletonBehavior<PoolsManager>
	{

		private Dictionary<string, IMemoryPool> poolsInGame = new Dictionary<string, IMemoryPool>();

		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();

			//this.CreatePool<GameObject>("item1",new BulltFactory());
			this.CreatePool<GameObject, BulltFactory>("item1");
			
		}

		class BulltFactory : IObjectFactory<GameObject>
		{
			GameObject IObjectFactory<GameObject>.Create()
			{
				var obj = Instantiate(PoolsManager.Instance.bulltObj );
				obj.SetActive(false);
				return obj;
			}

			void IObjectFactory<GameObject>.Release(GameObject go)
			{
				Destroy(go);
			}
		}

		public GameObject bulltObj;
		public float time = 1.0f;

		List<GameObject> objs = new List<GameObject>();

		public void Initalize()
		{
			
		}

		void Start()
		{

			
		}

		public IObjectPool<T> CreatePool<T>(string name, IObjectFactory<T> factory) where T : class
		{
			var pool = GetObjectPool<T>(name);
			if(pool == null)
			{
				if(factory != null)
				{
					pool = new SimpleObjectPool<T>(factory);
					poolsInGame.Add(name, pool);
				}
			}
			return pool;
		}

		public IMemoryPool CreatePool<T, U>(string name) 
			where T : class 
			where U : IObjectFactory<T>, new()
		{
			var pool = GetObjectPool<T>(name);
			if(pool == null)
			{
				pool = AutoPool<T, U>.Pool as IObjectPool<T>;
				poolsInGame.Add(name, pool);
			}
			return pool;
		}

		public IObjectPool<T> CreatePoolablePool<T>(string name, IObjectFactory<T> factory) where T : class, IPoolable
		{
			var pool = GetObjectPool<T>(name);
			if (pool == null)
			{
				if(factory != null)
				{
					pool = new PoolableObjectPool<T>(factory);
					poolsInGame.Add(name, pool);
				}
			}
			return pool;
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
			poolsInGame.ForEach	(
					(k, v) =>
					 {
						 v.ReleaseUnusedObjects();
					 }
				);
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

		public int count = 100;
		public int countMin = 0;

		private IEnumerator Alloc(float waitTime)
		{
			while(objs.Count < count)
			{
				var obj = Instance.GetObjectPool<GameObject>("item1").Allocate();
				obj.SetActive(true);
				objs.Add(obj);
				yield return new WaitForSeconds(waitTime);
			}
		}

		private IEnumerator Free(float waitTime)
		{
			while(objs.Count > countMin)
			{
				yield return new WaitForSeconds(waitTime);
				var obj = objs[0];
				objs.RemoveAt(0);
				obj.SetActive(false);
				Instance.GetObjectPool<GameObject>("item1").Recycle(obj);
			}
		}

		private void _Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				StartCoroutine(Alloc(time));
			}
			if(Input.GetKeyDown(KeyCode.B))
			{
				StartCoroutine(Free(time));
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				Instance.ReleaseAllUnusedPoolableObjects();
			}
		}
	}
}