using UnityEngine;
using Framework.Library.Singleton;
using Framework.Library.Log;


namespace Framework.Core
{
	[GameObjectPath("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class AssetsManager : ToSingletonBehavior<AssetsManager>
	{
		public void Initlize()
		{
			
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Numlock))
			{
				Debug.Log(ApplicationManager.Instance);
			}
		}
	}
}