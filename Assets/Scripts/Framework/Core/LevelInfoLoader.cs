
using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Client
{
	[DisallowMultipleComponent]
	public class LevelInfoLoader : ToSingletonBehavior<LevelInfoLoader>
	{
		protected override void OnSingletonInit()
		{
			
		}

		void LoadLevelAsync(string path, string levelName )
		{

		}
	}
}
