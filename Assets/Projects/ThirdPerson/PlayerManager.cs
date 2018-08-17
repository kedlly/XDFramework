using Framework.Library.Singleton;
using System;
using System.Collections.Generic;
using Protocal.Respond;
using Protocal;
using UnityEngine;
using Framework.Core.Runtime;

namespace Projects.ThirdPerson
{

	public enum PlayerType
	{
		Humam, Robot, UAV
	}

	public class Player
	{
		public int ID;
		public string Token;
		public PlayerType PType;

		public UnityEngine.Vector3 Position;
		public UnityEngine.Vector3 Velocity;
		public UnityEngine.Vector3 Direction;

		public GameObject go { get; private set; }
		static UnityEngine.Object resourceHuman;
		static UnityEngine.Object resourceUAV;
		static UnityEngine.Object resourceRobot;
		public Player()
		{
			
		}

		public void CreateGameObject()
		{
			UnityEngine.Object resource = null;
			switch (PType)
			{
				case PlayerType.Humam:
					if (resourceHuman == null)
					{
						resourceHuman = Resources.Load("Character/Player");
					}
					resource = resourceHuman;
					break;
				case PlayerType.UAV:
					if (resourceUAV == null)
					{
						resourceUAV = Resources.Load("Character/UAV");
					}
					resource = resourceUAV;
					break;
				case PlayerType.Robot:
					if (resourceUAV == null)
					{
						resourceUAV = Resources.Load("Character/Robot");
					}
					resource = resourceRobot;
					break;
				default:
					resource = null;
					break;
			}
			if (resource != null)
			{
				go = UnityEngine.GameObject.Instantiate(resource, Position, Quaternion.identity) as GameObject;
			}
			
		}

		public void RefreshMovment()
		{
			if (controller.MovementComp != null)
			{
				controller.MovementComp.Velocity = this.Velocity;
				controller.MovementComp.Position = this.Position;
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

		public void AttachCamera()
		{
			/*
			var camera = Camera.main;
			//camera.transform.parent = go.transform;
			camera.transform.localPosition = new UnityEngine.Vector3(0, 2.133f, -1.392f);
			camera.transform.localRotation = Quaternion.Euler(new UnityEngine.Vector3(27.942f, 0, 0));
			var src = go.AddComponent<SpringArmComponent>();
			src.hand = camera.transform;
			src.coefficientOfRestoringForce = 100;*/

			var camera = Camera.main;
			var sph = go.GetComponent<SpringArmComponent>();
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
	public class PlayerManager : ToSingleton<PlayerManager>
	{
		protected override void OnSingletonInit()
		{

		}

		private PlayerManager() { }

		Dictionary<int, Player> players = new Dictionary<int, Player>();

		public void AddPlayer(Player player)
		{
			players.Add(player.ID, player);
		}

		public void Drop(Player player)
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


		public Player Current { get; private set; }

		public void SetCurrentPlayer(Player player)
		{
			Current = player;
			AddPlayer(Current);
		}

		public Player GetPlayer(int id)
		{
			if (this.players.ContainsKey(id))
			{
				return this.players[id];
			}
			return null;
		}

		static void RPC_Respond_LoginAuth(ProtoBuf.IExtensible data)
		{
			Respond_LoginAuth rla = data as Respond_LoginAuth;
			if (rla != null)
			{
				Player player = new Player();
				player.ID = rla.player.pid;
				player.Token = rla.token;
				player.PType = rla.player.playerType == Protocal.RawData.PlayerType.EPT_HUMAN ? PlayerType.Humam :
					rla.player.playerType == Protocal.RawData.PlayerType.EPT_ROBOT ? PlayerType.Robot :
					rla.player.playerType == Protocal.RawData.PlayerType.EPT_UAV ? PlayerType.UAV : PlayerType.Humam;
				player.Position = rla.player.movement.position.PV2UV();
				player.Velocity = rla.player.movement.velocity.PV2UV();
				//player.Direction = new UnityEngine.Vector3(-0.02612079f, -1.65f, 3.404987f);
				if (PlayerManager.Instance.Current == null)
				{
					PlayerManager.Instance.SetCurrentPlayer(player);
					player.CreateGameObject();
					player.AttachCamera();
					player.AddLocalController();
				}
				foreach (var p in rla.neighborhood)
				{
					Player pc = new Player();
					pc.ID = p.pid;
					pc.Token = p.name;
					pc.PType = p.playerType == Protocal.RawData.PlayerType.EPT_HUMAN ? PlayerType.Humam :
								p.playerType == Protocal.RawData.PlayerType.EPT_ROBOT ? PlayerType.Robot :
								p.playerType == Protocal.RawData.PlayerType.EPT_UAV ? PlayerType.UAV : PlayerType.Humam;
					pc.Position = p.movement.position.PV2UV();
					pc.Velocity = p.movement.velocity.PV2UV();
					PlayerManager.Instance.AddPlayer(pc);
					pc.CreateGameObject();
					pc.AddRemoteController();
				}
			}
		}

		static void RPC_Respond_Moving(ProtoBuf.IExtensible data)
		{
			Respond_Moving rm = data as Respond_Moving;
			if (rm != null)
			{
				foreach (var m in rm.movementList)
				{
					var player = PlayerManager.Instance.GetPlayer(m.pid);
					if (player != null)
					{
						if (player != PlayerManager.Instance.Current)
						{
							player.Position = m.movement.position.toUV();
							player.Velocity = m.movement.velocity.toUV();
							player.RefreshMovment();
						}
						else
						{
							//TODO:
						}
					}
				}
			}
		}

		static void RPC_Respond_Logout(ProtoBuf.IExtensible data)
		{
			data.Process<Respond_Logout>(rl => 
			{
				var player = PlayerManager.Instance.GetPlayer(rl.pid);
				if (player != null)
				{
					if (player == PlayerManager.Instance.Current)
					{
						Application.Quit();
					}
					else
					{
						PlayerManager.Instance.Drop(player);
					}
				}
			});
		}

		

		static void RPC_Respond_PlayerAppeared(ProtoBuf.IExtensible data)
		{
			Respond_PlayerAppeared rpa = data as Respond_PlayerAppeared;
			if (rpa != null)
			{
				foreach (var m in rpa.neighborhood)
				{
					var player = PlayerManager.Instance.GetPlayer(m.pid);
					if (player == null)
					{
						player = new Player();
						player.ID = m.pid;
						player.Token = m.name;
						player.Position = m.movement.position.PV2UV();
						player.Velocity = m.movement.velocity.PV2UV();
						PlayerManager.Instance.AddPlayer(player);
						player.AddRemoteController();
					}
					player.Position = m.movement.position.toUV();
					player.Velocity = m.movement.velocity.toUV();
					player.RefreshMovment();
				}
			}
		}

	}


	public static class Helper
	{
		public static UnityEngine.Vector3 PV2UV(this Protocal.RawData.Vector3 vec)
		{
			return new UnityEngine.Vector3(vec.x, vec.y, vec.z);
		}
	}

}
