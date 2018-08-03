

using UnityEngine;

namespace Framework.Core
{
	public enum GameElementTypes
	{
		Character
	}

	public abstract class GameElement
	{
		public GameElementTypes ElementType { get; private set; }
		public GameElement(GameElementTypes type)
		{
			ElementType = type;
		}

		public GameObject AttachedGameObject { get; private set; }

		public void Attach(GameObject go)
		{
			AttachedGameObject = go;
		}
	}
}
