using System.Collections;
using System.Collections.Generic;

namespace Framework.Core.ReferenceCount
{
	public interface IResourceFactory<out T>
	{
		T Alloc();
		void Release();
	}
	public class RefrenceSource<T> : IRefCount where T : class
	{
		public T source = null;

		int IRefCount.RefCount
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		void IRefCount.Add()
		{
			throw new System.NotImplementedException();
		}

		void IRefCount.Release()
		{
			throw new System.NotImplementedException();
		}
	}

	public static class  RefrenceSourceManager
	{
		public static T GetSource<T>() where T : class, IRefCount
		{
			return null;
		}
	}
}