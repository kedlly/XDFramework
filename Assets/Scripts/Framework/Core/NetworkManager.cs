
using Framework.Utils.Extensions;
using Framework.Library.Singleton;
using UnityEngine;
using Framework.Network;
using System.Collections;
using System;
using System.Net.Sockets;

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
		AAsyncNetworkClient connector = new TcpNetworkClient();

		public void Initalize()
		{
		}

		private void OnDestroy()
		{
			if (connector != null)
			{
				connector.Dispose();
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
		{
			if (!connector.Closed)
			{
				connector.Send(data);
			}
		}

		public void SendNetworkMessage(ArraySegment<byte> data)
		{
			if (!connector.Closed)
			{
				connector.Send(data);
			}
		}

		public void Connect(string ip, int port)
		{
			connector.Connect(ip, port);
		}

		public event Action<ArraySegment<byte>> OnAsDataReceived
		{
			add
			{
				connector.onDataReceived += value;
			}
			remove
			{
				connector.onDataReceived -= value;
			}
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

		public event Action<SocketAsyncEventArgs> OnConnected
		{
			add
			{
				connector.onConnectCompleted += value;
			}
			remove
			{
				connector.onConnectCompleted -= value;
			}
		}
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

		public static void Send(this ArraySegment<byte> data)
		{
			if (data.Count == 0)
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
