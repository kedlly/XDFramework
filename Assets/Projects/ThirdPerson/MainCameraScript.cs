using Framework.Core;
using Protocal.Request;
using Protocal;
using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core.Runtime;

namespace Projects.ThirdPerson
{
	public class MainCameraScript : MonoBehaviour
	{
		public static Dictionary<Type, Delegate> methodList = new Dictionary<Type, Delegate>();
		private void Start()
		{
			GameConsole.Instance.OnCommand += cmd =>
			{
				switch (cmd)
				{
					case "1":
						cmd = "connect 127.0.0.1 8192";
						break;
					case "2":
						cmd = "login kedlly zhaoxionghui";
						break;
					default:
						break;
				}
				var cmds = cmd.Split(' ', '\t');
				
				if (cmds[0] == "connect")
				{
					var ip = cmds[1];
					var port = int.Parse(cmds[2]);
					Debug.Log("try to connect ip: " + ip + " port :" + port);
					NetworkManager.Instance.Connect(ip, port);
				}
				if (cmds[0] == "login")
				{
					var name = cmds[1];
					var password = cmds[2];
					var dataObject = new Request_LoginAuth();
					dataObject.username = name;
					dataObject.password = password;
					dataObject.Pack().Serialize().Send();
				}
				if (cmds[0] == "arm")
				{
					var src = PlayerManager.Instance.Current.go.GetComponent<SpringArmComponent>();
					if (src != null)
					{
						src.armRotateEnabled = !src.armRotateEnabled;
					}
				}
			};

			NetworkManager.Instance.OnDataReceived += data =>
			{
				var obj = data.Deserialize().Unpack();
				if (obj == null)
				{
					return;
				}
				if (!methodList.ContainsKey(obj.GetType()))
				{
					var namePath = obj.GetType().Name.Split('.');
					var methodName = "RPC_" + namePath[namePath.Length - 1];
					var method = typeof(PlayerManager).GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					Delegate mehthodDelegate = Delegate.CreateDelegate(typeof(ProtocalWrapper.MessageHandle), method);
					methodList[obj.GetType()] = mehthodDelegate;
				}
				var methodDelegate = methodList[obj.GetType()];
				methodDelegate.DynamicInvoke(obj);
			};

			NetworkManager.Instance.OnConnected += () =>
			{
				Debug.Log("connection build successful.");
			};
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				Application.Quit();
			}
			
		}
	}
}
