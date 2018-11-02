

namespace Framework.Library.ObjectPool
{

	using SimpleObjectPoolDefaultMemoryPolicy = Policies.DictionaryMemory;
	using PoolableObjectPoolDefaultMemoryPolicy = Policies.DictionaryMemory;

	public class SimpleObjectPool<T> 
		: ObjectPool<
				T
				, SimpleObjectPoolDefaultMemoryPolicy.PoolBufferPolicy<T>
			> 
		where T : class
	{
		public SimpleObjectPool(IObjectFactory<T> factory = null) : base(factory) { }
	}


	public class PoolableObjectPool<T> 
		: PoolableObjectPool<
				T
				, PoolableObjectPoolDefaultMemoryPolicy.PoolBufferPolicy<T>
			>
		where T : class , IPoolable
	{
		public PoolableObjectPool(IObjectFactory<T> factory = null) : base(factory) { }
	}


}