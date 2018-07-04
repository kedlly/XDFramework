using UnityEngine;
using System.Diagnostics;

namespace Framework.Core.Runtime
{
	public interface IEntity
	{

	}

	public class EntityWapper<T> : MonoBehaviour where T : IEntity
	{
		[Header("Data Definition")]
		public T Entity;
		private void Awake()
		{
			EntityManager.Instance.RegisterEntity<T>(Entity, this);
		}

		private void Start()
		{
			
		}

		private void OnAnimatorIK(int layerIndex)
		{
			
		}


	}
}