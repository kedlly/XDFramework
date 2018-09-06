


using System.Net.Sockets;
using System;
using System.Threading;
using System.Net;
using Framework.Utils.Extensions;
using Framework.Network.DataRecevier;
using Framework.Network.DataSender;

namespace Framework.Network
{

	public abstract class AAsyncNetworkClient : IDisposable
	{
		public bool Closed { get; private set; }
		/// <summary>
		/// 接收数据的缓冲区
		/// </summary>
		public byte[] ReceiveBuffer { get; private set; }
		/// <summary>
		/// 发送数据的缓冲区
		/// </summary>
		public byte[] SendBuffer { get; private set; }
		/// <summary>
		/// 客户端Socket对象
		/// </summary>
		public abstract Socket Socket { get; }
		/// <summary>
		/// 发送数据上下文对象
		/// </summary>
		public SocketAsyncEventArgs SendEventArgs { get; private set; }
		/// <summary>
		/// 接收数据上下文对象
		/// </summary>
		public SocketAsyncEventArgs ReceiveEventArgs { get; private set; }

		public abstract void ReCreateSocket();

		protected FSMDataReceiver receiver = new FSMDataReceiver();
		protected FSMDataSender sender = new FSMDataSender();

		public AAsyncNetworkClient(int recvBuffSize = 1024, int sendBuffSize = 1024)
		{
			ReceiveBuffer = new byte[recvBuffSize];//定义接收缓冲区
			SendBuffer = new byte[sendBuffSize];//定义发送缓冲区
			SendEventArgs = new SocketAsyncEventArgs();
			SendEventArgs.UserToken = this;
			ReceiveEventArgs = new SocketAsyncEventArgs();
			ReceiveEventArgs.UserToken = this;
			ReceiveEventArgs.SetBuffer(ReceiveBuffer, 0, ReceiveBuffer.Length);//设置接收缓冲区
			SendEventArgs.SetBuffer(SendBuffer, 0, SendBuffer.Length);//设置发送缓冲区
			SendEventArgs.Completed += Send_Completed;
			Closed = false;
		}

		private void _CloseSocket()
		{
			if (Socket != null)
			{
				ReceiveEventArgs.Completed -= Receive_Completed;
				ReceiveEventArgs.Completed -= Connect_Completed;
				CloseSocket();
				Closed = true;
			}
		}

		protected abstract void CloseSocket();

		public void Connect(string host, int port)
		{
			if (Socket.Connected)
			{
				_CloseSocket();
			}
			if (Closed)
			{
				ReCreateSocket();
			}
			Closed = false;
			var remoteEP = new IPEndPoint(IPAddress.Parse(host), port);
			ReceiveEventArgs.RemoteEndPoint = remoteEP;
			ReceiveEventArgs.Completed -= Connect_Completed;
			ReceiveEventArgs.Completed += Connect_Completed;
			Socket.ConnectAsync(ReceiveEventArgs);
		}

		public event Action<SocketAsyncEventArgs> onConnectCompleted;

		void Connect_Completed(object send, SocketAsyncEventArgs e)
		{
			ReceiveEventArgs.Completed -= Connect_Completed;
			if (e.SocketError == SocketError.Success)
			{
				ReceiveEventArgs.SetBuffer(ReceiveBuffer, 0, ReceiveBuffer.Length);
				ReceiveEventArgs.Completed -= Receive_Completed;
				ReceiveEventArgs.Completed += Receive_Completed;
				Socket.ReceiveAsync(ReceiveEventArgs);
			}
			if (onConnectCompleted != null)
			{
				onConnectCompleted(e);
			}
			
		}

		public event Action<ArraySegment<byte>> onDataReceived;
		void Receive_Completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred == 0)
			{
				//Console.WriteLine
				UnityEngine.Debug.Log("Socket is closed" + ",server {0} {1}".FormatEx(e.RemoteEndPoint, object.ReferenceEquals(e, ReceiveEventArgs)));
				_CloseSocket();
				ReceiveEventArgs.Completed -= Receive_Completed;
				if (onConnectCompleted != null)
				{
					e.SocketError = SocketError.Shutdown;
					onConnectCompleted(e);
				}
			}
			else
			{
				//put data to receiver's buffer
				receiver.ReceiveData(e.Buffer, 0, e.BytesTransferred);
				PickAllDataOneByOne();// read data from receiver's buffer
				Socket.ReceiveAsync(ReceiveEventArgs);
			}
		}

		
		private void PickAllDataOneByOne()
		{
			while (receiver.hasData && receiver.canRead)
			{
				var data = receiver.PickData();
				if (data.Count == 0)
				{
					throw new Exception("Error .. receiver.");
				}
				if (onDataReceived != null)
				{
					onDataReceived(data);
				}
			}
		}

		public void Send(byte[] message)
		{
			Send(new ArraySegment<byte>(message));
		}
		Thread sendingThread = null;
		ManualResetEvent resetEvent = new ManualResetEvent(true);
		ManualResetEvent senderCommandEvent = new ManualResetEvent(false);
		public void Send(ArraySegment<byte> message)
		{
			if (!Socket.Connected || Closed)
			{
				return;
			}
			try
			{
				sender.Append(message);
			}
			catch (RingBufferInSenderFull ex)
			{
				sender.sendBuffer.Resize(sender.sendBuffer.Capacity * 2);
				sender.Append(message);
			}
			
			if (sendingThread == null)
			{
				sendingThread = new Thread(thread_func);
				sendingThread.Start();
			}
			senderCommandEvent.Set();
		}

		private void thread_func()
		{
			while (true)
			{
				senderCommandEvent.WaitOne(2000);
				if (Closed)
				{
					break;
				}
				if (sender.hasData)
				{
					//resetEvent.WaitOne();
					//Console.WriteLine("");
					//Console.Write("data count in buffer: " + sender.sendBuffer.Count);
					var length = sender.TryPop(SendBuffer, 0);
					if (length != 0)
					{
						SendEventArgs.SetBuffer(0, length);
						//resetEvent.Reset();
						//Socket.SendAsync(SendEventArgs);
						Socket.Send(SendBuffer, 0, length, SocketFlags.None);
						//Console.Write("try to send: " + length + " left:" + sender.sendBuffer.Count);
					}
					else
					{ 
						senderCommandEvent.Reset();
					}
				}
				else
				{
					senderCommandEvent.Reset();
				}
			}
		}


		void Send_Completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred == 0)
			{
				//UnityEngine.Debug.Log("Socket is closed", Socket.Handle);
				_CloseSocket();
			}
			else
			{
				//doSend();
			}
			resetEvent.Set();
		}

		public abstract void Dispose();

		
	}

	
}