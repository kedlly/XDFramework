


using System.Net.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Net;
using System.IO;


public abstract class AAsyncConnector
{

	public delegate void ConnectCallback();


	private Thread clientBackgroundThread;


	private DataHolder mDataHolder = new DataHolder();

	Queue<byte[]> dataQueue = new Queue<byte[]>();

	Queue<Request> sendDataQueue = new Queue<Request>();

	private Socket socket;

	bool isStopReceive = true;

	//服务器IP地址  
	public string Host { get; private set; }

	//服务器端口  
	public int Port { get; private set; }

	public AAsyncConnector(string host, int port)
	{
		Host = host;
		Port = port;
	}

	public void Connect(int microsecondsTimeout)
	{
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
			dataQueue.Clear();
			sendDataQueue.Clear();
			isStopReceive = true;
			StartConnect((int)microsecondsTimeout);
		}
		catch (ThreadAbortException e)
		{
			Debug.Log("连接线程终止 " + e);
			Thread.ResetAbort();
		}
		catch (System.Exception ex)
		{
			Debug.Log("连接线程终止 " + ex);
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
			if (connectFailedCallback != null)
			{
				connectFailedCallback();
			}
		}
		else
		{
			//与socket建立连接成功，开启线程接受服务端数据。  
			isStopReceive = false;
			LoadProtocols();
			ReceiveSocket();
		}
		//线程结束
		Debug.Log("连接线程终止");
		clientBackgroundThread = null;
	}

	public event ConnectCallback connectCallback;
	public event ConnectCallback connectFailedCallback;

	private void _connectCallback(IAsyncResult iar)
	{
		if (iar.IsCompleted)
		{
			if (!socket.Connected)
			{
				if (connectFailedCallback != null)
				{
					connectFailedCallback();
				}
				return;
			}

			if (connectCallback != null)
			{
				connectCallback();
			}
		}
	}

	protected abstract Socket CreateSocket();


	protected abstract void LoadProtocols();

	#endregion

	private ManualResetEvent allDone = new ManualResetEvent(false);
	private void ReceiveSocket()
	{
		mDataHolder.Reset();
		while (!isStopReceive)
		{
			if (!socket.Connected)
			{
				//与服务器断开连接跳出循环  
				Debug.Log("Failed to clientSocket server.");
				socket.Close();
				break;
			}

			try
			{
				/*
				//接受数据保存至bytes当中  
				byte[] bytes = new byte[4096];
				//Receive方法中会一直等待服务端回发消息  
				//如果没有回发会一直在这里等着。  

				int i = socket.Receive(bytes);

				if (i <= 0)
				{
					socket.Close();
					break;
				}
				mDataHolder.PushData(bytes, i);

				while (mDataHolder.IsFinished())
				{
					dataQueue.Enqueue(mDataHolder.mRecvData);

					mDataHolder.RemoveFromHead();
				}
				*/

				byte[] bytes = new byte[4096];
				
			}
			catch (Exception e)
			{
				Debug.Log("Failed to clientSocket error." + e);
				socket.Close();
				break;
			}
		}
	}

	//接收到数据放入数据队列，按顺序取出
	void Update()
	{
		if (dataQueue.Count > 0)
		{
			//Resp resp = ProtoManager.Instance.TryDeserialize(dataQueue.Dequeue());
		}

		if (sendDataQueue.Count > 0)
		{
			Send();
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
		socket = null;
	}

	public bool isConnect()
	{
		return socket != null && socket.Connected;
	}

	public void SendMessage(Request req)
	{
		sendDataQueue.Enqueue(req);
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
			Request req = sendDataQueue.Dequeue();

			DataStream bufferWriter = new DataStream(true);
			req.Serialize(bufferWriter);
			byte[] msg = bufferWriter.ToByteArray();

			byte[] buffer = new byte[msg.Length + 4];
			DataStream writer = new DataStream(buffer, true);

			writer.WriteInt32((uint)msg.Length);//增加数据长度
			writer.WriteRaw(msg);

			byte[] data = writer.ToByteArray();

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

	private void SendCallback(IAsyncResult asyncConnect)
	{
		Debug.Log("send success");
	}

}

namespace Framework.System.Network.TCP
{

	public class ATcpConnector 
	{
		#region private members 	
		private TcpClient socketConnection;
		private Thread clientReceiveThread;
		#endregion

		public void ConnectToerver()
		{
			try
			{
				clientReceiveThread = new Thread(new ThreadStart(ListenForData));
				clientReceiveThread.IsBackground = true;
				clientReceiveThread.Start();
			}
			catch (Exception e)
			{
				Debug.Log("On client connect exception " + e);
			}
		}


		string ip;
		int port;

		public ATcpConnector()
		{

		}
		/// <summary> 	
		/// Runs in background clientReceiveThread; Listens for incomming data. 	
		/// </summary>     
		private void ListenForData()
		{
			try
			{
				socketConnection = new TcpClient(ip, port);
				Byte[] bytes = new Byte[1024];
				while (true)
				{
					// Get a stream object for reading 				
					using (NetworkStream stream = socketConnection.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 					
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);
							// Convert byte array to string message. 						
							string serverMessage = Encoding.ASCII.GetString(incommingData);
							Debug.Log("server message received as: " + serverMessage);
						}
					}
				}
			}
			catch (SocketException socketException)
			{
				Debug.Log("Socket exception: " + socketException);
			}
		}
		/// <summary> 	
		/// Send message to server using socket connection. 	
		/// </summary> 	
		private void SendMessage()
		{
			if (socketConnection == null)
			{
				return;
			}
			try
			{
				// Get a stream object for writing. 			
				NetworkStream stream = socketConnection.GetStream();
				if (stream.CanWrite)
				{
					string clientMessage = "This is a message from one of your clients.";
					// Convert string message to byte array.                 
					byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
					// Write byte array to socketConnection stream.                 
					stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
					Debug.Log("Client sent his message - should be received by server");
				}
			}
			catch (SocketException socketException)
			{
				Debug.Log("Socket exception: " + socketException);
			}
		}
	}
}


public class DataHolder
{
	public byte[] mRecvDataCache;//use array as buffer for efficiency consideration
	public byte[] mRecvData;

	private int mTail = -1;
	private int packLen;
	public void PushData(byte[] data, int length)
	{
		if (mRecvDataCache == null)
			mRecvDataCache = new byte[length];

		if (this.Count + length > this.Capacity)//current capacity is not enough, enlarge the cache
		{
			byte[] newArr = new byte[this.Count + length];
			mRecvDataCache.CopyTo(newArr, 0);
			mRecvDataCache = newArr;
		}

		Array.Copy(data, 0, mRecvDataCache, mTail + 1, length);
		mTail += length;
	}

	public bool IsFinished()
	{
		if (this.Count == 0)
		{
			//skip if no data is currently in the cache
			return false;
		}

		if (this.Count >= 4)
		{
			DataStream reader = new DataStream(mRecvDataCache, true);
			packLen = (int)reader.ReadInt32();
			if (packLen > 0)
			{
				if (this.Count - 4 >= packLen)
				{
					mRecvData = new byte[packLen];
					Array.Copy(mRecvDataCache, 4, mRecvData, 0, packLen);
					return true;
				}

				return false;
			}
			return false;
		}

		return false;
	}

	public void Reset()
	{
		mTail = -1;
	}

	public void RemoveFromHead()
	{
		int countToRemove = packLen + 4;
		if (countToRemove > 0 && this.Count - countToRemove > 0)
		{
			Array.Copy(mRecvDataCache, countToRemove, mRecvDataCache, 0, this.Count - countToRemove);
		}
		mTail -= countToRemove;
	}

	//cache capacity
	public int Capacity
	{
		get
		{
			return mRecvDataCache != null ? mRecvDataCache.Length : 0;
		}
	}

	//indicate how much data is currently in cache in bytes
	public int Count
	{
		get
		{
			return mTail + 1;
		}
	}
}

public class DataStream
{
	private BinaryReader mBinReader;
	private BinaryWriter mBinWriter;
	private MemoryStream mMemStream;
	private bool mBEMode;//big endian mode

	public DataStream(bool isBigEndian)
	{
		mMemStream = new MemoryStream();
		InitWithMemoryStream(mMemStream, isBigEndian);
	}

	public DataStream(byte[] buffer, bool isBigEndian)
	{
		mMemStream = new MemoryStream(buffer);
		InitWithMemoryStream(mMemStream, isBigEndian);
	}

	public DataStream(byte[] buffer, int index, int count, bool isBigEndian)
	{
		mMemStream = new MemoryStream(buffer, index, count);
		InitWithMemoryStream(mMemStream, isBigEndian);
	}

	private void InitWithMemoryStream(MemoryStream ms, bool isBigEndian)
	{
		mBinReader = new BinaryReader(mMemStream);
		mBinWriter = new BinaryWriter(mMemStream);
		mBEMode = isBigEndian;
	}

	public void Close()
	{
		mMemStream.Close();
		mBinReader.Close();
		mBinWriter.Close();
	}

	public void SetBigEndian(bool isBigEndian)
	{
		mBEMode = isBigEndian;
	}

	public bool IsBigEndian()
	{
		return mBEMode;
	}

	public long Position
	{
		get
		{
			return mMemStream.Position;
		}
		set
		{
			mMemStream.Position = value;
		}
	}

	public long Length
	{
		get
		{
			return mMemStream.Length;
		}
	}

	public byte[] ToByteArray()
	{
		//return mMemStream.GetBuffer();
		return mMemStream.ToArray();
	}



	public long Seek(long offset, SeekOrigin loc)
	{
		return mMemStream.Seek(offset, loc);
	}

	public void WriteRaw(byte[] bytes)
	{
		mBinWriter.Write(bytes);
	}

	public void WriteRaw(byte[] bytes, int offset, int count)
	{
		mBinWriter.Write(bytes, offset, count);
	}

	public void WriteByte(byte value)
	{
		mBinWriter.Write(value);
	}

	public byte ReadByte()
	{
		return mBinReader.ReadByte();
	}

	public void WriteInt16(UInt16 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public UInt16 ReadInt16()
	{
		UInt16 val = mBinReader.ReadUInt16();
		if (mBEMode)
			return BitConverter.ToUInt16(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	public void WriteInt32(UInt32 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public UInt32 ReadInt32()
	{
		UInt32 val = mBinReader.ReadUInt32();
		if (mBEMode)
			return BitConverter.ToUInt32(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	public void WriteInt64(UInt64 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public UInt64 ReadInt64()
	{
		UInt64 val = mBinReader.ReadUInt64();
		if (mBEMode)
			return BitConverter.ToUInt64(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	//public void WriteString8(string value)
	//{
	//    WriteInteger(BitConverter.GetBytes((byte)value.Length));
	//    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
	//    //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
	//    mBinWriter.Write(encoding.GetBytes(value));
	//}

	public void WriteString8(string value)
	{
		// System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] bytes = encoding.GetBytes(value);
		mBinWriter.Write((byte)bytes.Length);
		mBinWriter.Write(bytes);
	}

	public string ReadString8()
	{
		int len = ReadByte();
		byte[] bytes = mBinReader.ReadBytes(len);
		// System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		return encoding.GetString(bytes);
	}

	public void WriteString16(string value)
	{
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] data = encoding.GetBytes(value);
		WriteInteger(BitConverter.GetBytes((Int16)data.Length));
		//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		mBinWriter.Write(data);
	}

	public string ReadString16()
	{
		ushort len = ReadInt16();
		byte[] bytes = mBinReader.ReadBytes(len);
		//  System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		return encoding.GetString(bytes);
	}

	private void WriteInteger(byte[] bytes)
	{
		if (mBEMode)
			FlipBytes(bytes);
		mBinWriter.Write(bytes);
	}

	private byte[] FlipBytes(byte[] bytes)
	{
		//Array.Reverse(bytes); 
		for (int i = 0, j = bytes.Length - 1; i < j; ++i, --j)
		{
			byte temp = bytes[i];
			bytes[i] = bytes[j];
			bytes[j] = temp;
		}
		return bytes;
	}


	public void WriteSByte(sbyte value)
	{
		mBinWriter.Write(value);
	}

	public sbyte ReadSByte()
	{
		return mBinReader.ReadSByte();
	}

	public void WriteSInt16(Int16 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public Int16 ReadSInt16()
	{
		Int16 val = mBinReader.ReadInt16();
		if (mBEMode)
			return BitConverter.ToInt16(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	public void WriteSInt32(Int32 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public Int32 ReadSInt32()
	{
		Int32 val = mBinReader.ReadInt32();
		if (mBEMode)
			return BitConverter.ToInt32(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	public void WriteSInt64(Int64 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public Int64 ReadSInt64()
	{
		Int64 val = mBinReader.ReadInt64();
		if (mBEMode)
			return BitConverter.ToInt64(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}


	public void WriteUByte(byte value)
	{
		mBinWriter.Write(value);
	}

	public byte ReadUByte()
	{
		return mBinReader.ReadByte();
	}

	public void WriteUInt16(UInt16 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public UInt16 ReadUInt16()
	{
		UInt16 val = mBinReader.ReadUInt16();
		if (mBEMode)
			return BitConverter.ToUInt16(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	public void WriteUInt32(UInt32 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public UInt32 ReadUInt32()
	{
		UInt32 val = mBinReader.ReadUInt32();
		if (mBEMode)
			return BitConverter.ToUInt32(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}

	public void WriteUInt64(UInt64 value)
	{
		WriteInteger(BitConverter.GetBytes(value));
	}

	public UInt64 ReadUInt64()
	{
		UInt64 val = mBinReader.ReadUInt64();
		if (mBEMode)
			return BitConverter.ToUInt64(FlipBytes(BitConverter.GetBytes(val)), 0);
		return val;
	}
}

public abstract class Request
{

	public virtual int GetProtocol()
	{
		Debug.LogError("can't get Protocol");
		return 0;
	}
	public virtual void Serialize(DataStream writer)
	{
		writer.WriteSInt32(GetProtocol());
		writer.WriteByte(0);
	}

	public virtual void Deserialize(DataStream reader)
	{
		//no need to implement as this is a request
	}

	public void Send()
	{
		//SocketHelper.GetInstance().SendMessage(this);
	}
}

