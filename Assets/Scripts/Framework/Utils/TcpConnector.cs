using System;
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
			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
}
