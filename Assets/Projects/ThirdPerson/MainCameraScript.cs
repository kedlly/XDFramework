using Framework.Core;
using Protocol.Request;
using Protocol;
using System.Collections.Generic;
using UnityEngine;


namespace Projects.ThirdPerson
{
	public class MainCameraScript : MonoBehaviour
	{
		int index = 0;
		byte[] buffer = new byte[1024];
		Queue<ProtoBuf.IExtensible> networkData = new Queue<ProtoBuf.IExtensible>();
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
					
					System.ArraySegment<byte> p = new System.ArraySegment<byte>(buffer);
					dataObject.Pack().Serialize(p).Send();

					//dataObject.Pack().Serialize().Send();
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
				if (cmds[0] == "f")
				{
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "0123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "1OOOOOOOOOOOOOOO"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "2123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "3KKKKKKKKKKKKKKK"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "4123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "5TTTTTTTTTTTTTTT"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "6123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "7RRRRRRRRRRRRRRR"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "8123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "9MMMMMMMMMMMMMMM"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "a123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "bPPPPPPPPPPPPPPP"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "c123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "d123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "e123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "f123456789abcdef"))).Send();
					index++;
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

			byte[] bufferLast = new byte[512];
			byte[] bufferLast2 = new byte[512];

			NetworkManager.Instance.OnAsDataReceived += data =>
			{
				Debug.Log("thread id:" + System.Threading.Thread.CurrentThread.ManagedThreadId + "type:"  + "Count :" + data.Count);
				try
				{
					var obj = data.Deserialize().Unpack();
					if (obj == null)
					{
						return;
					}
					if (bufferLast != null)
					{
						byte[] q = bufferLast;
						System.Array.Copy(data.Array, data.Offset, q, 0, data.Count);
					}
					this.networkData.Enqueue(obj);
				}
				catch (System.Exception ex)
				{
					byte[] q = bufferLast2;
					byte[] p = bufferLast;
					System.Array.Copy(data.Array, data.Offset, q, 0, data.Count);
					
					print(ex.Message);
					throw;
				}
			};

			NetworkManager.Instance.OnConnected += (System.Net.Sockets.SocketAsyncEventArgs saea) =>
			{
				if (saea.SocketError == System.Net.Sockets.SocketError.Success)
				{
					Debug.Log("connection build succeed to Endpoint:" + saea.RemoteEndPoint);
				}
				else
				{
					Debug.LogErrorFormat("connection build failed to Endpoint: {0} Reason: {1}", saea.RemoteEndPoint , saea.SocketError);
				}
			};
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				Application.Quit();
			}
			if (networkData.Count > 0)
			{
				var obj = networkData.Dequeue();
				ProtocolProcessor.MessagePump(obj);
			}
		}
	}
}
