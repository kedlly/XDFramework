
using Framework.Library.ObjectPool.Policy;

namespace Framework.Library.ObjectPool
{
	public class SimpleObjectPool<T> : DictPoolBuffer<T> where T : class
	{
		public SimpleObjectPool(IObjectFactory<T> factory) : base(factory)
		{
		}
	}

}