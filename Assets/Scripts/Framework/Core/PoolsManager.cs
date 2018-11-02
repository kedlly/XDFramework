using UnityEngine;
using Framework.Library.Singleton;
using Framework.Library.ObjectPool;
using System.Collections.Generic;
using System.Collections;
using Framework.Utils.Extensions;
using Framework.Library.ObjectPool.Policies;

namespace Framework.Core
{
	[PathInHierarchy("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class PoolsManager : ToSingletonBehavior<PoolsManager>
	{

		private Dictionary<string, IMemoryPool> poolsInGame = new Dictionary<string, IMemoryPool>();

		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();

			//this.CreatePool<GameObject>("item1",new BulltFactory());
			//this.CreatePool<GameObject, BulltFactory>("item1");
			//this.CreatePool<GameObject>("item1", new BulltFactory());
			this.CreatePool<GameObject
					//, BulltFactory
					, Library.ObjectPool.Policies.DictionaryMemory.PoolBufferPolicy<GameObject>
				>("item1");
		}

		class PostObject : IPoolable
		{
			void IPoolable.OnAllocated()
			{
				Debug.Log("OnAllocated.....");
			}

			void IPoolable.OnRecycled()
			{
				Debug.Log("OnAllocated_____");
			}
			public GameObject go;
		}

		class BulltFactory : IObjectFactory<PostObject>
		{
			PostObject IObjectFactory<PostObject>.Create()
			{
				var obj = Instantiate(PoolsManager.Instance.bulltObj );
				obj.SetActive(false);
				return new PostObject() { go = obj };
			}

			void IObjectFactory<PostObject>.Release(PostObject go)
			{
				Destroy(go.go);
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

		private void Update()
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
			if (Input.GetKeyDown(KeyCode.D))
			{
				System.GC.Collect();
			}
		}
	}
}