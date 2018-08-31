


using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;


namespace Framework.Utils
{
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