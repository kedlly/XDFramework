
using Framework.Utils.Extensions;
using Framework.Library.Singleton;
using UnityEngine;
using Framework.Utils;
using System.Net.Sockets;
using System;

namespace Framework.Core
{

	[PathInHierarchy("/[Game]/NetworkManager"), DisallowMultipleComponent]
	public sealed class NetworkManager : ToSingletonBehavior<NetworkManager>
	{
		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();
		}

		//AAsyncConnector connector = new TcpConnector();
		AAsyncNetworkClient connector = new GameTcpClient();

		public void Initalize()
		{
		}

		private void OnDestroy()
		{
			if (connector != null)
			{
				//connector.CloseSocket();
			}
		}

		private void Update()
		{
			if (connector != null)
			{
				//connector.Update();
			}
		}

		public void SendNetworkMessage(byte[] data)
		{/*
			if (connector != null && connector.isConnect())
			{
				int length = data.Length;
				
				byte[] lengthBit = System.BitConverter.GetBytes(length);
				if (System.BitConverter.IsLittleEndian)
				{
					Array.Reverse(lengthBit);
				}
				byte[] newData = new byte[lengthBit.Length + length];
				Array.Copy(lengthBit, newData, lengthBit.Length);
				Array.Copy(data, 0, newData, lengthBit.Length, data.Length);
				connector.SendMessage(newData);
			}*/
			if (!connector.Closed)
			{
				connector.Send(data);
			}
		}

		public void Connect(string ip, int port)
		{
			connector.Connect(ip, port);
		}

		public event Action<byte[]> OnDataReceived;/*
		{
			add
			{
				(connector as TcpConnector).OnDataReceived += value;
			}
			remove
			{
				(connector as TcpConnector).OnDataReceived -= value;
			}
		}*/

		public event Action OnConnected; /*
		{
			add
			{
				connector.ConnectCallback += value;
			}
			remove
			{
				connector.ConnectCallback -= value;
			}
		}*/
	}

	public static class NetworkHelper
	{
		public static void Send(this byte[] data)
		{
			if (data == null)
			{
				Debug.LogWarning("ignore send null or zero length data uses networkHelper .");
			}
			else
			{
				NetworkManager.Instance.SendNetworkMessage(data);
			}
		}
	}
}
