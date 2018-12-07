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

		protected override int TotalObjectCount { get { return totalObjectCount; } }

		public StackPolicy(IObjectFactory<T> factory = null) : base(factory) { }

		private int totalObjectCount = 0;

		protected override T OnAllocate(T template)
		{
			T obj = null;
			if (_stackPool.Count > 0)
			{
				obj = _stackPool.Pop();
			}
			else
			{
				obj = ObjectFactory.Create();
				ObjectFactory.Copy(obj, template);
				totalObjectCount++;
			}
			return obj;
		}

		protected override bool OnRecycle(T obj)
		{
			_stackPool.Push(obj);
			return true;
		}

		protected override void OnReleaseUnusedObjects(int count)
		{
			count = count == -1 || count > _stackPool.Count ? _stackPool.Count : count;
			while (count > 0)
			{
				var obj = _stackPool.Pop();
				ObjectFactory.Release(obj);
				count--;
			}
		}

		protected override void OnReleaseAllObjects()
		{
			OnReleaseUnusedObjects(-1);
			totalObjectCount = 0;
		}
	}


	public class PoolBufferPolicy<T> : StackPolicy<T> where T : class
	{
		public PoolBufferPolicy(IObjectFactory<T> factory = null) : base(factory) { }
	}
}
