using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Core.Runtime
{
	public class EntityManager : ToSingleton<EntityManager>
	{
		protected override void OnSingletonInit()
		{
			
		}

		private EntityManager()
		{

		}

		public void RegisterEntity<T>(IEntity entity, MonoBehaviour behavior) 
		{

		}

		public EntityWapper<T>[] Get<T>() where T : IEntity
		{
			return new EntityWapper<T>[] { };
		}
	}
}