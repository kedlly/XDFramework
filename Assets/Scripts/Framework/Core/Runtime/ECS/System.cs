using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Core.Runtime.ECS
{
	public interface ISystem
	{
		void Initialize();
		void Tick();
		void Release();
	}

	public class IEntitySystem
	{
		
	}
	


}