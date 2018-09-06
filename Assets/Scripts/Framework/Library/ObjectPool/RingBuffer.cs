using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library
{
	public class RingBufferException : Exception
	{
		public RingBufferException(string msg) : base(msg) { }
	}

	public class RBLowMemoryException : RingBufferException
	{
		public RBLowMemoryException() : base("buffer has not enough space to store new data.") { }
		public RBLowMemoryException(int maxLength, int insertLength) 
			: base(string.Format("buffer can insert max data count : {0}, but {1}", maxLength -1, insertLength)) { }
	}

	public class RBDataReadException : RingBufferException
	{
		public RBDataReadException() : base("buffer has not enough data to read.") { }
		public RBDataReadException(int maxLength, int readLength)
			: base(string.Format("buffer can read max data count : {0}, but {1}", maxLength - 1, readLength)) { }
	}


	public class RingBuffer<T>
	{
		private T[] _buffer;
		private int _startIndex, _endIndex;

		public int Capacity { get { return _buffer.Length; } }

		public RingBuffer(int size)
		{
			_buffer = new T[Unsafe.GetPower2(size)];
		}

		internal static int CircleWriteProc(T[] buffer, int writeIndex, int readIndex, T[] data, int offset, int len)
		{
			return CircleWriteProc(buffer, writeIndex, readIndex, new ArraySegment<T>(data, offset, len < 0 ? data.Length : len));
		}

		internal static int CircleWriteProc(T[] buffer
										, int writeIndex  /* 0 -> length-1 为写索引位置*/
										, int readIndex   /* 0 -> length, 等于读索引位置 或 buffer 边界， 当为边界时 必然有第二次写操作，并在其中重置写索引位置*/
										, ArraySegment<T> data)
		{
			if (buffer == null)
			{
				throw new ArgumentException("buffer must be not null.");
			}
			if (writeIndex >= buffer.Length )
			{
				throw new ArgumentException("writeIndex must be less then buffer.length.");
			}
			// 若data中无数据,直接返回
			if (data.Count == 0)
			{
				return writeIndex;
			}
			//计算读写索引之间的距离
			int distance = readIndex - writeIndex;
			//buffer中可写区域计算
			int bufferMaxWriteCount = distance > 0 ? distance : buffer.Length + distance;
			
			if (data.Count > bufferMaxWriteCount)
			{
				throw new RBLowMemoryException();
			}

			if (data.Count > buffer.Length - 1)
			{
				throw new RBLowMemoryException(buffer.Length, data.Count);
			}
			
			int newDataIndex = writeIndex;
			
			if (distance > 0) // 读索引在前， 写索引在后
			{
				Array.Copy(data.Array, data.Offset, buffer, writeIndex, data.Count);
				newDataIndex = writeIndex + data.Count;
				if (newDataIndex > buffer.Length)
				{
					throw new Exception("cannot calc write index");
				}
				newDataIndex &= (buffer.Length - 1);
			}
			else
			{
				int lastSpace = buffer.Length - writeIndex;
				if (lastSpace >= data.Count)
				{
					newDataIndex = CircleWriteProc(buffer, writeIndex, writeIndex + data.Count, data);
				}
				else
				{
					ArraySegment<T> directCopyPart = new ArraySegment<T>(data.Array, data.Offset, lastSpace);
					newDataIndex = CircleWriteProc(buffer, writeIndex, writeIndex + directCopyPart.Count, directCopyPart);
					int remainedDataLength = data.Count - directCopyPart.Count;
					ArraySegment<T> remainedCopyPart = new ArraySegment<T>(data.Array, data.Offset + directCopyPart.Count, remainedDataLength);
					newDataIndex = CircleWriteProc(buffer, 0, readIndex, remainedCopyPart);
				}
				
			}

			return newDataIndex;
		}

		internal static int CircleReadProc(T[] buffer, int readIndex, int writeIndex, T[] data, int offset, int len)
		{
			return CircleReadProc(buffer, readIndex, writeIndex, new ArraySegment<T>(data, offset, len < 0 ? data.Length : len));
		}

		internal static int CircleReadProc(T[] buffer, int readIndex, int writeIndex, ArraySegment<T> data)
		{
			int distance = writeIndex - readIndex;	
			int bufferMaxReadCount = distance > 0 ? distance : buffer.Length + distance;

			if (data.Count > bufferMaxReadCount)
			{
				throw new RBDataReadException();
			}
			if (data.Count == 0)
			{
				return readIndex;
			}

			if (data.Count > buffer.Length - 1)
			{
				throw new RBDataReadException(buffer.Length, data.Count);
			}

			int newDataIndex = readIndex;
			
			if (distance > 0) // 读索引在后， 写索引在前
			{
				Array.Copy(buffer, readIndex, data.Array, data.Offset, data.Count);
				newDataIndex = readIndex + data.Count;
				if (newDataIndex > buffer.Length)
				{
					throw new Exception("cannot calc read index");
				}
				newDataIndex &= (buffer.Length - 1);
			}
			else
			{
				int lastSpace = buffer.Length - readIndex;
				if (lastSpace >= data.Count)
				{
					newDataIndex = CircleReadProc(buffer, readIndex, readIndex + data.Count, data);
				}
				else
				{
					ArraySegment<T> directCopyPart = new ArraySegment<T>(data.Array, data.Offset, lastSpace);
					newDataIndex = CircleReadProc(buffer, readIndex, readIndex + directCopyPart.Count, directCopyPart);
					int remainedDataLength = data.Count - directCopyPart.Count;
					ArraySegment<T> remainedCopyPart = new ArraySegment<T>(data.Array, data.Offset + directCopyPart.Count, remainedDataLength);
					newDataIndex = CircleReadProc(buffer, 0, readIndex, remainedCopyPart);
				}
			}

			return newDataIndex;
		}

		public int Count
		{
			get
			{
				if (_endIndex > _startIndex)
					return _endIndex - _startIndex;
				if (_endIndex < _startIndex)
					return (_buffer.Length - _startIndex) + _endIndex;
				return 0;
			}
		}

		public void Write(T[] data, int offset = 0, int dataCount = -1)
		{
			if (dataCount < 0)
			{
				dataCount = data.Length;
			}
			this.Write(new ArraySegment<T>(data, offset, dataCount));
		}

		public void Write(ArraySegment<T> data)
		{
			_endIndex = CircleWriteProc(this._buffer, this._endIndex, this._startIndex, data);
		}

		public T[] Read(int len, bool keepData = false)
		{
			var result = new T[len < 0 ? 0 : len];
			if (result.Length != 0)
			{
				var newIndex = CircleReadProc(this._buffer, _startIndex, _endIndex, result, 0, result.Length);
				if (!keepData)
				{
					_startIndex = newIndex;
				}
			}
			return result;
			
		}

		public void Read(T[] buffer, int startIndex, int len, bool keepData = false)
		{
			var newIndex = CircleReadProc(this._buffer, _startIndex, _endIndex, buffer, startIndex, len);
			if (!keepData)
			{
				_startIndex = newIndex;
			}
		}

		public void Read(ArraySegment<T> buffer, bool keepData = false)
		{
			var newIndex = CircleReadProc(this._buffer, _startIndex, _endIndex, buffer);
			if (!keepData)
			{
				_startIndex = newIndex;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= Count)
					throw new ArgumentOutOfRangeException();
				return _buffer[(_startIndex + index) & (_buffer.Length - 1)];
			}
		}

		public IEnumerable<T> Items
		{
			get
			{
				for (var i = 0; i < Count; i++)
					yield return _buffer[(_startIndex + i) & (_buffer.Length - 1)];
			}
		}

		public void Resize(int size)
		{
			int newSize = Unsafe.GetPower2(size);
			var count = _endIndex - _startIndex;
			count = count >= 0 ? count : _buffer.Length + count;
			if (count < newSize)
			{
				var newBuffer = new T[newSize];
				RingBuffer<T>.CircleReadProc(_buffer, _startIndex, _endIndex, newBuffer, 0, count);
				_startIndex = 0;
				_endIndex = count;
				_buffer = newBuffer;
			}
		}
	}


	public class SafeRingBuffer<T>
	{
		private T[] _buffer;
		private int _startIndex, _endIndex;

		public int Capacity { get { return _buffer.Length; } }

		public SafeRingBuffer(int size)
		{
			_buffer = new T[Unsafe.GetPower2(size)];
		}

		public int Count
		{
			get
			{
				lock(this)
				{
					if (_endIndex > _startIndex)
						return _endIndex - _startIndex;
					if (_endIndex < _startIndex)
						return (_buffer.Length - _startIndex) + _endIndex;
					return 0;
				}
			}
		}

		public void Write(T[] data, int offset = 0, int dataCount = -1)
		{
			if (dataCount < 0)
			{
				dataCount = data.Length;
			}
			this.Write(new ArraySegment<T>(data, offset, dataCount));
		}

		public void Write(ArraySegment<T> data)
		{
			lock(this)
			{
				_endIndex = RingBuffer<T>.CircleWriteProc(this._buffer, this._endIndex, this._startIndex, data);
			}
		}

		public T[] Read(int len, bool keepData = false)
		{
			lock(this)
			{
				var result = new T[len < 0 ? 0 : len];
				if (result.Length != 0)
				{
					var newIndex = RingBuffer<T>.CircleReadProc(this._buffer, _startIndex, _endIndex, result, 0, result.Length);
					if (!keepData)
					{
						_startIndex = newIndex;
					}
				}
				return result;
			}
		}

		public void Read(T[] buffer, int startIndex, int len, bool keepData = false)
		{
			lock (this)
			{
				var newIndex = RingBuffer<T>.CircleReadProc(this._buffer, _startIndex, _endIndex, buffer, startIndex, len);
				if (!keepData)
				{
					_startIndex = newIndex;
				}
			}
		}

		public void Read(ArraySegment<T> buffer, bool keepData = false)
		{
			lock(this)
			{
				var newIndex = RingBuffer<T>.CircleReadProc(this._buffer, _startIndex, _endIndex, buffer);
				if (!keepData)
				{
					_startIndex = newIndex;
				}
			}
		}

		public void Resize(int size)
		{
			lock (this)
			{
				int newSize = Unsafe.GetPower2(size);
				var count = _endIndex - _startIndex;
				count = count >= 0 ? count : _buffer.Length + count; 
				if (count < newSize)
				{
					var newBuffer = new T[newSize];
					RingBuffer<T>.CircleReadProc(_buffer, _startIndex, _endIndex, newBuffer, 0, count);
					_startIndex = 0;
					_endIndex = count;
					_buffer = newBuffer;
				}
			}
		}
	}

}
