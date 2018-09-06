using Framework.Library;
using Framework.Utils.Extensions;
using System;

namespace Framework.Network.DataSender
{
	public class RingBufferInSenderFull : Exception
	{
		public RingBufferInSenderFull() : base("RingBuffer is Full") {}
	}
	public class FSMDataSender
	{
		enum State
		{
			ReadLength
			, ReadData
		}

		public RingBuffer<byte> sendBuffer { get; private set; }
		public FSMDataSender(int ringBufferSzie = 1024)
		{
			sendBuffer = new RingBuffer<byte>(ringBufferSzie);
		}


		public void Append(byte[] data)
		{
			Append(new ArraySegment<byte>(data));
		}

		private byte[] appendBuffer = new byte[128];

		public void Append(ArraySegment<byte> data)
		{
			lock (this)
			{
				if (appendBuffer.Length < data.Count + 8)
				{
					appendBuffer = new byte[data.Count + 8];
				}
				// 写入辅助长度
				Unsafe.FillBytes_Int(data.Count + 4, appendBuffer);
				// 写入实际长度
				Unsafe.FillBytes_Int(data.Count, appendBuffer, 4);
				if (System.BitConverter.IsLittleEndian)
				{
					Array.Reverse(appendBuffer, 4, 4);
				}
				Array.Copy(data.Array, data.Offset, appendBuffer, 8, data.Count);
				var segment = new ArraySegment<byte>(appendBuffer, 0, data.Count + 8);
				if (sendBuffer.Capacity < sendBuffer.Count + segment.Count)
				{
					//数据池已经放不下这个序列
					throw new RingBufferInSenderFull();//Exception("send buffer is full.");
				}
				sendBuffer.Write(segment);
			}
		}


		public int TryPop(byte[] buffer, int startIndex)
		{
			lock (this)
			{
				var length = 0;
				while (hasData && (readDataLength() + length) <= buffer.Length)
				{
					var len = Pop(buffer, startIndex + length);
					if (len == 0)
					{
						throw new Exception();
					}
					length += len;
				}
				return length;
			}
		}


		State _state = State.ReadLength;

		int dataLength = 0;
		int remainedPopLen = 0;


		private byte[] lengthBuffer = new byte[4];

		private int readDataLength()
		{
			sendBuffer.Read(lengthBuffer, 0, lengthBuffer.Length, true);
			return System.BitConverter.ToInt32(lengthBuffer, 0);
		}
		private void dropLengthSection()
		{
			sendBuffer.Read(lengthBuffer, 0, lengthBuffer.Length);
		}

		internal int Pop(byte[] buffer, int startIndex)
		{
			if (_state == State.ReadLength)
			{
				if (sendBuffer.Count <= 4)
				{
					throw new Exception("Error buffer.");
				}
				dataLength = readDataLength();
				remainedPopLen = dataLength;
				if (remainedPopLen <= buffer.Length - startIndex)
				{
					sendBuffer.Read(lengthBuffer, 0, 4);
					_state = State.ReadData;
				}
				else
				{
					//buffer 长度必须足够，否则会引起异常
					throw new Exception("FSMDataSender cannot pop data to buffer that length = {0}, statrIndex at {1}, but data length = {2}"
						.FormatEx(buffer.Length, startIndex, remainedPopLen));
				}

			}
			if (_state == State.ReadData)
			{
				var lenOfPopData = Math.Min(remainedPopLen, buffer.Length - startIndex);
				sendBuffer.Read(buffer, startIndex, lenOfPopData);
				remainedPopLen = remainedPopLen - lenOfPopData;
				if (remainedPopLen == 0)
				{
					_state = State.ReadLength;
				}
				return lenOfPopData;
			}
			return 0;
		}

		

		

		internal ArraySegment<byte> Pop(ArraySegment<byte> buffer)
		{
			if (_state == State.ReadLength)
			{
				if (sendBuffer.Count <= 4)
				{
					throw new Exception("Error buffer.");
				}
				dataLength = readDataLength();
				remainedPopLen = dataLength;
				if (remainedPopLen <= buffer.Count)
				{
					dropLengthSection();
					_state = State.ReadData;
				}
				else
				{
					//buffer 长度必须足够，否则会引起异常
					throw new Exception("FSMDataSender cannot pop data to buffer that length = {0}, but data length = {1}"
						.FormatEx(buffer.Count, remainedPopLen));
				}

			}
			if (_state == State.ReadData)
			{
				var lenOfPopData = Math.Min(remainedPopLen, buffer.Count);
				ArraySegment<byte> result = new ArraySegment<byte>(buffer.Array, buffer.Offset, lenOfPopData);
				sendBuffer.Read(result);
				remainedPopLen = remainedPopLen - lenOfPopData;
				if (remainedPopLen == 0)
				{
					_state = State.ReadLength;
				}
				return result;
			}
			return default(ArraySegment<byte>);
		}

		public bool hasData { get { return sendBuffer.Count > 4 && sendBuffer.Count >= 4 + readDataLength(); } }

		
	}
}
