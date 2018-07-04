using Framework.Library.Singleton;

namespace Framework.Client.GameRules
{

	public class LevelLogic : ToSingleton<LevelLogic>
	{
		private LevelLogic() {  }


		protected override void OnSingletonInit()
		{
			UnityEngine.Debug.Log("------------");
		}

		protected override void OnSingletonReborn()
		{
			UnityEngine.Debug.Log("))))))))))");
		}
	}


}
