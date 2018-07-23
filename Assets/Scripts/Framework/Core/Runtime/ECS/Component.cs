using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Core.Runtime.ECS
{

	public struct AFA : IEntity
	{
		public int stest;
	}

	public class UUSC : EntityComponentWapper<AFA, UUSC>
	{

	}

}