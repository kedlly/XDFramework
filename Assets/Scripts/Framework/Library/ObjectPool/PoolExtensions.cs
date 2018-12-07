
using Framework.Library.ObjectPool.Policies;
using Framework.Utils.Extensions;
using System;



namespace Framework.Library.ObjectPool
{
	public static class PoolExtensions
	{
		public static IObjectPool<T> CreatePool<T>(this IObjectFactory<T> factory) where T : class
		{
			Type ObjectType = typeof(T);
			if (ObjectType.ImplementsInterface<IPoolable>())
			{
				var poolType = typeof(PoolableObjectPool<>).CreateGenericClassType(ObjectType);
				return Activator.CreateInstance(poolType, factory) as IObjectPool<T>;
			}
			else
			{
				return new SimpleObjectPool<T>(factory);
			}
		}

		public static IObjectPool<TObject> CreatePool<TObject, TBufferPolicy>(this IObjectFactory<TObject> factory)
			where TObject : class
			where TBufferPolicy : BufferPolicy<TObject>
		{
			Type ObjectType = typeof(TObject);
			if (ObjectType.ImplementsInterface<IPoolable>())
			{
				var poolType = typeof(PoolableObjectPool<,>).CreateGenericClassType(ObjectType, typeof(TBufferPolicy));
				return Activator.CreateInstance(poolType, factory) as IObjectPool<TObject>;
			}
			else
			{
				return new ObjectPool<TObject, TBufferPolicy>(factory);
			}
		}

		public static IObjectPool<T> CreateSharedPool<T>(this IObjectFactory<T> factory) where T : class
		{
			return new SharedObjectPool<T, SharedBufferPolicy<T>>(factory);
		}
	}

}