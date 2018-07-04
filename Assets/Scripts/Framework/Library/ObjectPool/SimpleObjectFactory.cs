

namespace Framework.Library.ObjectPool
{
	public class SimpleObjectFactory<T> : IObjectFactory<T> where T : class, new()
	{
		T IObjectFactory<T>.Create()
		{
			return new T();
		}

		void IObjectFactory<T>.Release(T obj)
		{
			throw new System.NotImplementedException();
		}
	}

}