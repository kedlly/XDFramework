using System.Collections;
using System.Collections.Generic;


namespace Framework.Core.ReferenceCount
{
	public interface IRefCount
	{
		int RefCount { get; }
		void Add();
		void Release();
	}
	
}