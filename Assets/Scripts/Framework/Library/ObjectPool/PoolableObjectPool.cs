

namespace Framework.Library.ObjectPool
{

	public class PoolableObjectPool<T> : SimpleObjectPool<T> where T : class, IPoolable
	{
		public PoolableObjectPool(IObjectFactory<T> factory) : base (factory)
		{
		}

		public override T Allocate()
		{
			T allocated = base.Allocate();
			if (allocated != null)
			{
				allocated.OnAllocate();
			}
			return allocated;
		}

		public override bool Recycle(T obj)
		{
			bool succeed = base.Recycle(obj);
			if (succeed)
			{
				obj.OnRecycle();
			}
			return succeed;
		}
	}
}
