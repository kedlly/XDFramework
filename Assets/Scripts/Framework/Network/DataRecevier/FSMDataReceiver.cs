
using Framework.Library;
using System;

namespace Framework.Network.DataRecevier
{

	public class FSMDataReceiver
	{
		public RingBuffer<byte> receiveBuffer { get; private set; }

		public FSMDataReceiver(int ringBufferSize = 2048)
		{
			receiveBuffer = new RingBuffer<byte>(ringBufferSize);
		}

		enum State
		{
			ReadLength
			, ReadData
		}

		State _state = State.ReadLength;

		public void ReceiveData(byte[] originalData)
		{
			ReceiveData(new ArraySegment<byte>(originalData));
		}

		public void ReceiveData(byte[] originalData, int offset, int length)
		{
			ReceiveData(new ArraySegment<byte>(originalData, offset, length));
		}

		int length = 0;
		byte[] lengthDataBuffer = new byte[4];
		int lengthDataRemainedCount = 4;

		public bool hasData { get { return receiveBuffer.Count > 0; } }
		private int GetReadDataLength()
		{
			receiveBuffer.Read(lengthDataBuffer, 0, lengthDataBuffer.Length, true);
			var length = System.BitConverter.ToInt32(lengthDataBuffer, 0);
			return length;
		}

		private void DropReadDataLength()
		{
			receiveBuffer.Read(lengthDataBuffer, 0, lengthDataBuffer.Length);
		}

		bool IsDataReadable()
		{
			var length = GetReadDataLength();
			return length <= receiveBuffer.Count - 4;
		}

		public bool canRead { get { return IsDataReadable(); } }

		byte[] receiverDataBuffer = new byte[128];
		public ArraySegment<byte> PickData()
		{
			if (hasData && canRead)
			{
				var length = GetReadDataLength();
				DropReadDataLength();
				if (length > receiverDataBuffer.Length)
				{
					receiverDataBuffer = new byte[length];
				}
				var result = new ArraySegment<byte>(receiverDataBuffer, 0, length);
				receiveBuffer.Read(result);
				return result;
			}
			return default(ArraySegment<byte>);
		}

		public void ReceiveData(ArraySegment<byte> segment)
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
