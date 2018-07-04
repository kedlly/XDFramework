using Framework.Library.ObjectPool;
using Framework.Utils.Extensions;
using System;


static class AutoPool<T, Factory>
	where Factory : IObjectFactory<T>, new()
	where T : class
{

	static volatile IMemoryPool instance;
	private static readonly object _lock = new object();


	public static IMemoryPool Pool
	{
		get
		{
			if (instance != null)
			{
				return instance;
			}
			lock(_lock)
			{
				instance = Create();
				return instance;
			}
		}
	}

	public static bool IsPoolable
	{
		get
		{
			return typeof(T).ImplementsInterface<IPoolable>();
		}
	}

	static IMemoryPool createPoolable(Factory factory)
	{
		Type poolType = typeof(PoolableObjectPool<>);
		Type[] typeArgs = { typeof(T) };
		var makeme = poolType.MakeGenericType(typeArgs);
		return (IMemoryPool)Activator.CreateInstance(makeme, factory);
	}

	static IMemoryPool Create()
	{
		var factory = new Factory();
		return IsPoolable ? createPoolable(factory) : new SimpleObjectPool<T>(factory);
	}

}



public class ToAutoPool<T, U> 
	where T : class 
	where U : IObjectFactory<T>, new()
{
	public static IMemoryPool Pool
	{
		get
		{
			return AutoPool<T, U>.Pool;
		}
	}
}