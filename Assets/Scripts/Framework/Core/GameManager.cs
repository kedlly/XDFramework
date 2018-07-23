using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;

namespace Framework.Core
{

	[GameObjectPath("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class GameManager : ToSingletonBehavior<GameManager>
	{
		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();
		}

		public void Initalize()
		{
			AssetsManager.Instance.Initalize();
			PoolsManager.Instance.Initalize();
		}

		public void Update()
		{
			FlowControl.FlowControlMananger.Instance.Tick();
		}
	}
}