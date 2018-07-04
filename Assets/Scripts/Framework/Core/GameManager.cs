using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Core
{

	[GameObjectPath("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class GameManager : ToSingletonBehavior<GameManager>
	{
		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();
		}

		public void Initlize()
		{
			AssetsManager.Instance.Initlize();
			PoolsManager.Instance.Initlize();
		}

		public void Update()
		{
			FlowControl.FlowControlMananger.Instance.Tick();
		}
	}
}