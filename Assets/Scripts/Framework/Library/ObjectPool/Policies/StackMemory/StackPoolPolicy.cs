using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library.ObjectPool.Policies.StackMemory
{
	public class StackPolicy<T> : BufferPolicy<T> where T : class
	{
	
		public Stack<T> _stackPool = new Stack<T>();

		protected override int UnusedObjectCount { get { return _stackPool.Count; } }

		protected override int TotalObjectCount { get { return _stackPool.Count; } }

		public StackPolicy(IObjectFactory<T> factory = null) : base(factory) { }

		protected override T OnAllocate()
		{
			return _stackPool.Count > 0 ? _stackPool.Pop() : ObjectFactory.Create();
		}

		protected override bool OnRecycle(T obj)
		{
			_stackPool.Push(obj);
			return true;
		}

		protected override void OnReleaseUnusedObjects()
		{
			while (_stackPool.Count > 0)
			{
				var obj = _stackPool.Pop();
				ObjectFactory.Release(obj);
			}
		}

		protected override void OnReleaseAllObjects()
		{
			OnReleaseUnusedObjects();
		}
	}


	public class PoolBufferPolicy<T> : StackPolicy<T> where T : class
	{
		public PoolBufferPolicy(IObjectFactory<T> factory = null) : base(factory) { }
	}
}
