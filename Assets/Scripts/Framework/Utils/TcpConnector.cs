﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Framework.Utils
{
	public class TcpConnector : AAsyncConnector
	{

		public TcpConnector()
		{

		}

		protected override Socket CreateSocket()
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
			return socket;
		}

		protected override void LoadProtocols()
		{
			//throw new NotImplementedException();
		}

		protected override void OnReceivedData(byte[] data)
		{
			if (OnDataReceived != null)
			{
				OnDataReceived(data);
			}
		}

		public event Action<byte[]> OnDataReceived;

	}

	public class GameTcpClient : AAsyncNetworkClient
	{
		public override Socket Socket
		{
			get
			{
				if (_socket == null)
				{
					_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
				}
				return _socket;
			}
		}

		private Socket _socket;
	}
}
