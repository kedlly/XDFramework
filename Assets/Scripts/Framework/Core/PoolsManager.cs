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
		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();

			//this.CreatePool<GameObject>("item1",new BulltFactory());
			//this.CreatePool<GameObject, BulltFactory>("item1");
			//this.CreatePool<GameObject>("item1", new BulltFactory());
			this.CreatePool<GameObject
					//, BulltFactory
					, Library.ObjectPool.Policies.DictionaryMemory.DictionaryPolicy<GameObject>
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
			public void Copy(PostObject target, PostObject template)
			{
				throw new System.NotImplementedException();
			}

			public PostObject Create()
			{
				throw new System.NotImplementedException();
			}

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
			return FrameworkUtils.MemoryPoolManager.CreatePool<T>(name, factory);
		}

		public IObjectPool<TObject> CreatePool<TObject, TFactory, TBufferPolicy>(string name)
			where TFactory : IObjectFactory<TObject>, new()
			where TBufferPolicy : BufferPolicy<TObject>
			where TObject : class
		{
			return FrameworkUtils.MemoryPoolManager.CreatePool<TObject, TFactory, TBufferPolicy>(name);
		}

		public IObjectPool<TObject> CreatePool<TObject, TBufferPolicy>(string name)
			where TBufferPolicy : BufferPolicy<TObject>
			where TObject : class
		{
			return FrameworkUtils.MemoryPoolManager.CreatePool<TObject, TBufferPolicy>(name);
		}

		public IObjectPool<T> GetObjectPool<T>(string name) where T : class
		{
			return FrameworkUtils.MemoryPoolManager.GetObjectPool<T>(name);
		}

		public void ReleaseAllUnusedPoolableObjects()
		{
			FrameworkUtils.MemoryPoolManager.ReleaseAllUnusedPoolableObjects();
		}

		public void ReleasePool<T>(string name) where T : class
		{
			FrameworkUtils.MemoryPoolManager.ReleasePool<T>(name);
		}

		public IObjectPool<T> CreateSharedPool<T>(string name) where T : class
		{
			return FrameworkUtils.MemoryPoolManager.CreateSharedPool<T>(name);
		}

		public int count = 100;
		public int countMin = 0;

		public GameObject firstobj = null;
		private IEnumerator Alloc(float waitTime)
		{
			while(objs.Count < count)
			{
				Instance.GetObjectPool<GameObject>("item1").MaxDeepth = 8;
				var obj = Instance.GetObjectPool<GameObject>("item1").Allocate();
				if (firstobj == null)
				{
					firstobj = obj;
				}
				obj.SetActive(true);
				objs.Add(obj);
				//Debug.Log("+" + objs.Count);
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
				//Instance.GetObjectPool<GameObject>("item1").ReserveDeepth = 6;
				Instance.GetObjectPool<GameObject>("item1").Recycle(obj);
				//Debug.Log("-" + objs.Count);
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