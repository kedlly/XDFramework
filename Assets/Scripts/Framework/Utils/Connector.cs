


using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net;
using Framework.Library;

namespace Framework.Utils
{

	public abstract class AAsyncNetworkClient
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
		}

		public void Connect(string host, int port)
		{
			var remoteEP = new IPEndPoint(IPAddress.Parse(host), port);
			ReceiveEventArgs.RemoteEndPoint = remoteEP;
			ReceiveEventArgs.Completed -= Connect_Completed;
			ReceiveEventArgs.Completed += Connect_Completed;
			Socket.ConnectAsync(ReceiveEventArgs);
		}

		public event Action<SocketAsyncEventArgs> onConnectCompleted;
		

		void Connect_Completed(object send, SocketAsyncEventArgs e)
		{
			Console.WriteLine("Is connected to server {0}", e.RemoteEndPoint);
			ReceiveEventArgs.Completed -= Connect_Completed;
			if (e.SocketError == SocketError.Success)
			{
				ReceiveEventArgs.SetBuffer(ReceiveBuffer, 0, ReceiveBuffer.Length);
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
				Console.WriteLine("Socket is closed", Socket.Handle);
				Socket.Close();
				Closed = true;
			}
			else
			{
				receiver.Read(e.Buffer, 0, e.BytesTransferred);
				checkReceiveData();
				Socket.ReceiveAsync(ReceiveEventArgs);
			}
		}

		byte[] receiverLengthDataBuffer = new byte[4];
		byte[] receiverDataBuffer = new byte[128];
		private void checkReceiveData()
		{
			while (receiver.receiveBuffer.Count > 0)
			{
				receiver.receiveBuffer.Read(receiverLengthDataBuffer, 0, receiverLengthDataBuffer.Length, true);
				var length = System.BitConverter.ToInt32(receiverLengthDataBuffer, 0);
				if (length > receiver.receiveBuffer.Count - 4)
				{
					break;
				}
				receiver.receiveBuffer.Read(receiverLengthDataBuffer, 0, 4);
				if (length > receiverDataBuffer.Length)
				{
					receiverDataBuffer = new byte[length];
				}
				receiver.receiveBuffer.Read(receiverDataBuffer, 0, length);
				if (onDataReceived != null)
				{
					onDataReceived(new ArraySegment<byte>(receiverDataBuffer, 0, length));
				}
			}
		}



		protected virtual void ReceiveData(byte[] data) { }

		public void Send(byte[] message)
		{
			if (!Socket.Connected)
			{
				return;
			}
			sender.Append(message);
			if (sender.hasData)
			{
				var length = sender.Pop(SendBuffer, 0);
				SendEventArgs.SetBuffer(0, length);
				Socket.SendAsync(SendEventArgs);
			}
		}

		void Send_Completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred == 0)
			{
				Console.WriteLine("Socket is closed", Socket.Handle);
				Socket.Close();
				Closed = true;
			}
			else
			{
				Console.WriteLine("sent {0} byte(s) data to server.", e.BytesTransferred);
				if (this.sender.hasData)
				{
					Socket.SendAsync(SendEventArgs);
				}
			}
		}

		enum State
		{
			ReadLength
				, ReadData
		}

		public class FSMDataSender
		{
			public RingBuffer<byte> senderBuffer = new RingBuffer<byte>(2048);
			public void Append(byte[] data)
			{
				int length = data.Length;
				if (senderBuffer.Capacity < senderBuffer.Count + 4 * 2 + length)
				{
					//数据池已经放不下这个序列
					throw new Exception("send buffer is full.");
				}
				FillBytes(length + 4, lengthBuffer);
				// 写入辅助长度
				senderBuffer.Write(lengthBuffer);
				// 写入实际长度
				FillBytes(length, lengthBuffer);
				if (System.BitConverter.IsLittleEndian)
				{
					Array.Reverse(lengthBuffer);
				}
				senderBuffer.Write(lengthBuffer);
				// 写入数据
				senderBuffer.Write(data);
			}

			State _state = State.ReadLength;

			int dataLength = 0;
			int remainedPopLen = 0;

			public int Pop(byte[] buffer, int startIndex)
			{
				if (_state == State.ReadLength)
				{
					if (senderBuffer.Count <= 4)
					{
						throw new Exception("Error buffer.");
					}
					senderBuffer.Read(lengthBuffer, 0, 4);
					dataLength = System.BitConverter.ToInt32(lengthBuffer, 0);
					remainedPopLen = dataLength;
					_state = State.ReadData;
				}
				if (_state == State.ReadData)
				{
					var lenOfPopData = Math.Min(remainedPopLen, buffer.Length - startIndex);
					senderBuffer.Read(buffer, startIndex, lenOfPopData);
					remainedPopLen = remainedPopLen - lenOfPopData;
					if (remainedPopLen == 0)
					{
						_state = State.ReadLength;
					}
					return lenOfPopData;
				}
				return 0;
			}

			public bool hasData { get { return senderBuffer.Count > 0; } }

			private byte[] lengthBuffer = new byte[4];
			public static unsafe void FillBytes(int value, byte[] buffer, int index = 0)
			{
				byte[] bytes = buffer;
				fixed (byte* b = &bytes[index])
					*((int*)b) = value;
				return;
			}
		}


		public class FSMDataReceiver
		{
			public readonly RingBuffer<byte> receiveBuffer = new RingBuffer<byte>(2048);
			

			State _state = State.ReadLength;

			public void Read(byte[] originalData)
			{
				Read(new ArraySegment<byte>(originalData));
			}

			public void Read(byte[] originalData, int offset, int length)
			{
				Read(new ArraySegment<byte>(originalData, offset, length));
			}

			int length = 0;
			byte[] lengthDataBuffer = new byte[4];
			int lengthDataRemainedCount = 4;

			public void Read(ArraySegment<byte> segment)
			{
				var remained = segment;
				while (remained.Count > 0)
				{
					if (_state == State.ReadLength)
					{
						ArraySegment<byte> lengthSeg = doRead(remained, lengthDataRemainedCount, out remained);
						Array.Copy(lengthSeg.Array, lengthSeg.Offset, lengthDataBuffer, lengthDataBuffer.Length - lengthDataRemainedCount, lengthSeg.Count);
						lengthDataRemainedCount = lengthDataRemainedCount - lengthSeg.Count;
						if (lengthDataRemainedCount == 0)
						{
							if (System.BitConverter.IsLittleEndian)
							{
								Array.Reverse(lengthDataBuffer);
							}
							length = System.BitConverter.ToInt32(lengthDataBuffer, 0);
							receiveBuffer.Write(lengthDataBuffer);
							_state = State.ReadData;
						}
					}
					if (_state == State.ReadData)
					{
						if (remained.Count > 0)
						{
							if (remained.Count >= length)
							{
								var dataSegment = doRead(remained, length, out remained);
								receiveBuffer.Write(dataSegment.Array, dataSegment.Offset, dataSegment.Count);
								length = 0;
								_state = State.ReadLength;
								lengthDataRemainedCount = lengthDataBuffer.Length;
							}
							else
							{
								receiveBuffer.Write(remained.Array, remained.Offset, remained.Count);
								length -= remained.Count;
								remained = new ArraySegment<byte>(remained.Array, remained.Offset + remained.Count, 0);
							}
						}
					}
				}

				if (remained.Count > 0)
				{
					throw new Exception("------------");
				}
			}

			private static ArraySegment<byte> doRead(ArraySegment<byte> data, int length, out ArraySegment<byte> remainedSegment)
			{
				remainedSegment = new ArraySegment<byte>(data.Array, 0, 0);
				ArraySegment<byte> result = remainedSegment;
				int actualLength = Math.Min(length, data.Count);
				result = new ArraySegment<byte>(data.Array, data.Offset, actualLength);
				remainedSegment = new ArraySegment<byte>(data.Array, data.Offset + actualLength, data.Count - actualLength);
				return result;
			}
		}
	}

	public abstract class AAsyncConnector
	{

		private Thread clientBackgroundThread;

		private Socket socket;

		protected Queue<byte[]> dataReceived = new Queue<byte[]>();
		protected Queue<byte[]> dataSent = new Queue<byte[]>();

		protected bool isStopReceive = true;

		//服务器IP地址  
		public string Host { get; private set; }

		//服务器端口  
		public int Port { get; private set; }

		public AAsyncConnector()
		{
			
		}

		public void Connect(string host, int port, int microsecondsTimeout)
		{
			Host = host;
			Port = port;

			socket = CreateSocket();
			try
			{
				if (clientBackgroundThread != null)
				{
					clientBackgroundThread.Abort();
				}
				clientBackgroundThread = new Thread(new ParameterizedThreadStart(StartConnect));
				clientBackgroundThread.IsBackground = true;
				clientBackgroundThread.Start(microsecondsTimeout);
			}
			catch (Exception e)
			{
				Debug.Log("On client connect exception " + e);
			}
		}

		#region run in background thread
		private void StartConnect(object microsecondsTimeout)
		{
			try
			{
				isStopReceive = true;
				StartConnect((int)microsecondsTimeout);
			}
			catch (ThreadAbortException e)
			{
				Debug.Log("connect thread terminated" + e);
				//Thread.ResetAbort();
			}
			catch (System.Exception ex)
			{
				Debug.Log("connect thread terminated " + ex);
			}

		}

		private void StartConnect(int microsecondsTimeout)
		{
			socket = CreateSocket();
			var result = socket.BeginConnect(Host, Port, new AsyncCallback(_connectCallback), socket);
			bool success = result.AsyncWaitHandle.WaitOne(microsecondsTimeout, true);
			if (!success)
			{
				//超时  
				CloseSocket();
				if (ConnectFailedCallback != null)
				{
					ConnectFailedCallback();
				}
			}
			else
			{
				//与socket建立连接成功，开启线程接受服务端数据。  
				// connection done
				isStopReceive = false;
				LoadProtocols();
				//socket.Send(System.Text.Encoding.Default.GetBytes("Hello"));
				ReceiveSocket();
// 				while (socket.Connected)
// 				{
// 					Thread.Sleep(100);
// 				}
			}
			//线程结束
			Debug.Log("connection thread quit.");
			clientBackgroundThread = null;
		}

		public event Action ConnectCallback;
		public event Action ConnectFailedCallback;

		private void _connectCallback(IAsyncResult iar)
		{
			if (iar.IsCompleted)
			{
				if (!socket.Connected)
				{
					if (ConnectFailedCallback != null)
					{
						ConnectFailedCallback();
					}
					return;
				}

				if (ConnectCallback != null)
				{
					ConnectCallback();
				}
			}
		}

		protected abstract Socket CreateSocket();


		protected abstract void LoadProtocols();

		#endregion

		///private ManualResetEvent allDone = new ManualResetEvent(false);
		//接受数据保存至bytes当中  
		byte[] bytes = new byte[8192];
		//Receive方法中会一直等待服务端回发消息  
		//如果没有回发会一直在这里等着。  
		private void ReceiveSocket()
		{
			while (!isStopReceive)
			{
				if (!socket.Connected)
				{
					//与服务器断开连接跳出循环  
					Debug.Log("Failed to client Socket server.");
					socket.Close();
					break;
				}

				try
				{
					int i = socket.Receive(bytes);
					if (i <= 0)
					{
						socket.Close();
						break;
					}
					var buff = new byte[i];
					Array.Copy(bytes, buff, i);
					dataReceived.Enqueue(buff);
				}
				catch (Exception e)
				{
					Debug.Log("Failed to clientSocket error." + e);
					socket.Close();
					break;
				}
			}
		}


		//关闭Socket  
		public void CloseSocket()
		{
			isStopReceive = true;
			if (socket != null && socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			if (socket != null)
				((System.IDisposable)socket).Dispose();
			socket = null;
			if (clientBackgroundThread != null && clientBackgroundThread.IsAlive)
			{
				clientBackgroundThread.Abort();
			}
		}

		public bool isConnect()
		{
			return socket != null && socket.Connected;
		}

		public void SendMessage(byte[] data)
		{
			dataSent.Enqueue(data);
		}

		private void Send()
		{
			if (socket == null)
			{
				return;
			}
			if (!socket.Connected)
			{
				return;
			}
			try
			{
				var data = dataSent.Dequeue();
				IAsyncResult asyncSend = socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
				bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
				if (!success)
				{
					//Closed();
				}
			}
			catch (Exception e)
			{
				Debug.Log("send error : " + e.ToString());
			}
		}

		public void Update()
		{
			if (dataSent.Count > 0)
			{
				Send();
			}
			else
			{
				if (dataReceived.Count > 0)
				{
					OnReceivedData(dataReceived.Dequeue());
				}
			}
		}

		protected abstract void OnReceivedData(byte[] data);

		protected virtual void SendCallback(IAsyncResult asyncConnect)
		{
			//Debug.Log("send success");
		}
	}

	
}