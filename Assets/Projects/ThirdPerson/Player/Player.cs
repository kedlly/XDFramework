﻿using Framework.Core.Runtime;
using UnityEngine;


namespace Projects.ThirdPerson
{
	public enum GamePlayerType
	{
		Humam, Robot, UAV
	}

	public static class PlayerInfoHelper
	{
		
		public static Protocal.RawData.PlayerType ToProtoPlayerType(this GamePlayerType pt)
		{
			Protocal.RawData.PlayerType value = Protocal.RawData.PlayerType.EPT_HUMAN;
			switch (pt)
			{
				case GamePlayerType.Humam:
					value = Protocal.RawData.PlayerType.EPT_HUMAN;
					break;
				case GamePlayerType.Robot:
					value = Protocal.RawData.PlayerType.EPT_ROBOT;
					break;
				case GamePlayerType.UAV:
					value = Protocal.RawData.PlayerType.EPT_UAV;
					break;
				default:
					break;
			}
			return value;
			//Protocal.RawData.PlayerType.EPT_HUMAN? PlayerType.Humam:
			//rla.player.playerType == Protocal.RawData.PlayerType.EPT_ROBOT ? PlayerType.Robot :
			//rla.player.playerType == Protocal.RawData.PlayerType.EPT_UAV ? PlayerType.UAV : PlayerType.Humam;
		}

		public static GamePlayerType ToGamePlayerType(this Protocal.RawData.PlayerType pt)
		{
			GamePlayerType value = GamePlayerType.Humam;
			switch (pt)
			{
				case Protocal.RawData.PlayerType.EPT_HUMAN:
					value = GamePlayerType.Humam;
					break;
				case Protocal.RawData.PlayerType.EPT_ROBOT:
					value = GamePlayerType.Robot;
					break;
				case Protocal.RawData.PlayerType.EPT_UAV:
					value = GamePlayerType.UAV;
					break;
				default:
					break;
			}
			return value;
		}
	}


	public class GamePlayerInfo
	{
		public int ID { get; private set; }
		public string Token { get; private set; }
		public string Name { get; set; }
		public GamePlayerType PlayerType { get; private set; }
		public Vector3 Position { get; set; }
		public Vector3 Velocity { get; set; }
		public Vector3 Direction { get; set; }

		public GamePlayerInfo(int id, string token = "", string name = "",GamePlayerType type = GamePlayerType.Humam)
		{
			this.ID = id;
			this.Token = token;
			this.PlayerType = type;
			this.Name = name;
		}

		public void Attach(GamePlayerProxy proxy = null)
		{
			if (proxy != null)
			{
				this.proxy = proxy;
			}
			else
			{
				this.proxy = new GamePlayerProxy(this);
			}
		}

		GamePlayerProxy proxy;

		public void Login()
		{
			if (proxy != null)
			{
				if (proxy.go == null)
				{
					proxy.CreateGameObject();
				}
				proxy.AttachCamera();
				proxy.AddLocalController();
			}
		}

		public void Appear()
		{
			if (proxy != null)
			{
				if (proxy.go == null)
				{
					proxy.CreateGameObject();
				}
				proxy.AddRemoteController();
			}
		}

		public void RefreshMovment()
		{
			if (proxy != null)
			{
				proxy.RefreshMovment();
			}
		}

		public void Drop()
		{
			if (proxy != null)
			{
				proxy.Drop();
			}
		}
	}

	public class GamePlayerProxy
	{
		public GameObject go { get; private set; }
		public GamePlayerInfo PlayerInfo { get; private set; }
		public GamePlayerProxy(GamePlayerInfo playerInfo)
		{
			this.PlayerInfo = playerInfo;
		}

		public void CreateGameObject()
		{
			Object resource = ResourceMgr.GetRes(PlayerInfo.PlayerType);
			
			if (resource != null)
			{
				go = GameObject.Instantiate(resource, PlayerInfo.Position, Quaternion.identity) as GameObject;
			}

		}

		public void RefreshMovment()
		{
			if (controller.MovementComp != null)
			{
				controller.MovementComp.Velocity = PlayerInfo.Velocity;
				controller.MovementComp.Position = PlayerInfo.Position;
			}
		}

		ActorControllerComponent controller = null;

		private void AddController<T>() where T : ActorControllerComponent
		{
			if (go != null)
			{
				if (controller != null)
				{
					UnityEngine.Object.Destroy(controller);
					controller = null;
				}
				controller = go.AddComponent<T>();
				controller.player = this;
			}
		}

		public void AddLocalController()
		{
			AddController<LocalControllerComponent>();
		}

		public void AddRemoteController()
		{
			AddController<RemoteControllerComponent>();
		}

		public void AttachCamera(Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			var sph = go.GetComponentInChildren<SpringArmComponent>();
			if (sph != null)
			{
				if (sph.hand != null)
				{
					camera.transform.parent = sph.hand.transform;
					camera.transform.localEulerAngles = Vector3.zero;
					camera.transform.localPosition = Vector3.zero;
					camera.transform.localScale = Vector3.one;
				}
				else
				{
					sph.hand = camera.transform;
				}
				sph.enabled = true;
			}
		}

		public void Drop()
		{
			if (go != null)
			{
				UnityEngine.Object.Destroy(go);
			}
		}
	}
}
