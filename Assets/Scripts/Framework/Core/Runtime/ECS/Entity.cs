using UnityEngine;
using System.Diagnostics;

namespace Framework.Core.Runtime.ECS
{
	public interface IEntity
	{

	}

	public class EntityComponentWapper<TEntity, TOwnerSystem> : MonoBehaviour 
		where TEntity : IEntity
	{
		[Header("Data Definition")]
		public TEntity Entity;
	}

	public class EntitySystemWrapper<TEntity> : MonoBehaviour
	{

	}
}