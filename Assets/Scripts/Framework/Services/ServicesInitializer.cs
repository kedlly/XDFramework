

using UnityEngine;

namespace Framework.Services
{
	static class ServicesRuntimeInitialize
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void InitPositioningService()
		{
			//Positioning.PositioningServiceInitializer.Setup();
		}
	}
}
