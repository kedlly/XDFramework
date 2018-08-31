using Framework.Core;
using Protocol.Request;
using Protocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core.Runtime;

namespace Projects.ThirdPerson
{
	public class MainCameraScript : MonoBehaviour
	{
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
					case "3":
						cmd = "login uav 123456";
						break;
					case "4":
						cmd = "login robot 123456";
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
				if (cmds[0] == "quality")
				{
					QualitySettings.SetQualityLevel(int.Parse(cmds[1]));
				}
				if (cmds[0] == "print")
				{
					if (cmds[1] == "frameTime")
					{
						Debug.Log("deltaTime :"+Time.deltaTime + " fixedDeltaTime "+ Time.fixedDeltaTime);
					}
				}
			};

			ProtocolProcessor.Register<NetworkCommunication>();
			NetworkManager.Instance.OnDataReceived += data =>
			{

				var obj = data.Deserialize().Unpack();
				if (obj == null)
				{
					return;
				}
				ProtocolProcessor.MessagePump(obj);
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
