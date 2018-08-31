using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library
{
	public class RingBuffer<T>
	{
		private readonly T[] _buffer;
		private int _startIndex, _endIndex;

		public int Capacity { get { return _buffer.Length; } }

		public RingBuffer(int size)
		{
			_buffer = new T[size];
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
			if (dataCount == -1)
			{
				dataCount = data.Length;
			}
			if (Count + dataCount > _buffer.Length)
				throw new Exception("buffer overflow");
			if (_endIndex + dataCount >= _buffer.Length)
			{
				var endLen = _buffer.Length - _endIndex;
				var remainingLen = dataCount - endLen;

				Array.Copy(data, offset, _buffer, _endIndex, endLen);
				Array.Copy(data, endLen, _buffer, 0, remainingLen);
				_endIndex = remainingLen;
			}
			else
			{
				Array.Copy(data, offset, _buffer, _endIndex, dataCount);
				_endIndex += dataCount;
			}
		}

		public T[] Read(int len, bool keepData = false)
		{
			if (len > Count)
				throw new Exception("not enough data in buffer");
			var result = new T[len];
			if (_startIndex + len < _buffer.Length)
			{
				Array.Copy(_buffer, _startIndex, result, 0, len);
				if (!keepData)
					_startIndex += len;
				return result;
			}
			else
			{
				var endLen = _buffer.Length - _startIndex;
				var remainingLen = len - endLen;
				Array.Copy(_buffer, _startIndex, result, 0, endLen);
				Array.Copy(_buffer, 0, result, endLen, remainingLen);
				if (!keepData)
					_startIndex = remainingLen;
				return result;
			}
		}

		public void Read(T[] buffer, int startIndex, int len, bool keepData = false)
		{
			if (len > Count)
				throw new Exception("not enough data in buffer");
			if (_startIndex + len < _buffer.Length)
			{
				Array.Copy(_buffer, _startIndex, buffer, startIndex, len);
				if (!keepData)
					_startIndex += len;
			}
			else
			{
				var endLen = _buffer.Length - _startIndex;
				var remainingLen = len - endLen;
				Array.Copy(_buffer, _startIndex, buffer, startIndex, endLen);
				Array.Copy(_buffer, 0, buffer, startIndex + endLen, remainingLen);
				if (!keepData)
					_startIndex = remainingLen;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= Count)
					throw new ArgumentOutOfRangeException();
				return _buffer[(_startIndex + index) % _buffer.Length];
			}
		}

		public IEnumerable<T> Items
		{
			get
			{
				for (var i = 0; i < Count; i++)
					yield return _buffer[(_startIndex + i) % _buffer.Length];
			}
		}
	}

}
