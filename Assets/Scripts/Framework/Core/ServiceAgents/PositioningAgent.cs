using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework.Library.Singleton;

namespace Framework.Client.ServiceAgents
{
	[RequireComponent(typeof(LevelInfoLoader))]
	[DisallowMultipleComponent]
	public class PositioningAgent : ToSingletonBehavior<PositioningAgent>
	{
		public void SetPlayerPositionInLevel(int playerId, Vector3 postion)
		{
			
		}

		protected override void OnSingletonInit()
		{
			
		}
	}
}
