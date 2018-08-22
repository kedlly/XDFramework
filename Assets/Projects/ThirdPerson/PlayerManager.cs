using Framework.Library.Singleton;
using System;
using System.Collections.Generic;
using Protocal.Respond;
using Protocal;
using UnityEngine;
using Framework.Core.Runtime;

namespace Projects.ThirdPerson
{
	public class PlayerManager : ToSingleton<PlayerManager>
	{
		protected override void OnSingletonInit()
		{

		}

		private PlayerManager() { }

		Dictionary<int, GamePlayerInfo> players = new Dictionary<int, GamePlayerInfo>();

		public void AddPlayer(GamePlayerInfo player)
		{
			players.Add(player.ID, player);
			player.Attach();
		}

		public void Drop(GamePlayerInfo player)
		{
			Drop(player.ID);
		}

		public void Drop(int pid)
		{
			if (players.ContainsKey(pid))
			{
				var player = players[pid];
				if (player != null)
				{
					player.Drop();
				}
				players.Remove(pid);
			}
		}


		public GamePlayerInfo Current { get; private set; }

		public void SetCurrentPlayer(GamePlayerInfo player)
		{
			Current = player;
			AddPlayer(Current);
		}

		public GamePlayerInfo GetPlayer(int id)
		{
			if (this.players.ContainsKey(id))
			{
				return this.players[id];
			}
			return null;
		}



	}
}
