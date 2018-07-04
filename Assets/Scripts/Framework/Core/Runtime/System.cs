using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Core.Runtime
{
	public interface ISystem 
	{
		void Initialize();
		void Tick();
		void Release();
	}


	
}