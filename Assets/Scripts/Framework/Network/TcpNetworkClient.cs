using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Framework.Network
{
	public class TcpNetworkClient : AAsyncNetworkClient
	{
		public override Socket Socket
		{
			get
			{
				if (_socket == null)
				{
					ReCreateSocket();
				}
				return _socket;
			}
		}

		private Socket _socket;

		public override void Dispose()
		{
			TryCloseSocket();
		}

		public override void ReCreateSocket()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
			_socket.SendBufferSize = 1024;
			_socket.ReceiveBufferSize = 1024;
		}

		private void TryCloseSocket()
		{
			if (_socket != null)
			{
				_socket.Close();
				_socket = null;
			}
		}

		protected override void CloseSocket()
		{
			TryCloseSocket();
		}
	}
}
