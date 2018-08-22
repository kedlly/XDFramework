
using UnityEngine;

namespace Projects.ThirdPerson
{
	public class ResourceMgr
	{
		static Object resourceHuman;
		static Object resourceUAV;
		static Object resourceRobot;

		public static Object GetRes(GamePlayerType type)
		{
			Object resource = null;
			switch (type)
			{
				case GamePlayerType.Humam:
					if (resourceHuman == null)
					{
						resourceHuman = Resources.Load("Character/Player");
					}
					resource = resourceHuman;
					break;
				case GamePlayerType.UAV:
					if (resourceUAV == null)
					{
						resourceUAV = Resources.Load("Character/UAV");
					}
					resource = resourceUAV;
					break;
				case GamePlayerType.Robot:
					if (resourceRobot == null)
					{
						resourceRobot = Resources.Load("Character/Robot");
					}
					resource = resourceRobot;
					break;
				default:
					resource = null;
					break;
			}
			return resource;
		}
	}

	
}
