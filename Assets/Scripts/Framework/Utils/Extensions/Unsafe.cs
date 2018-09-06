using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utils.Extensions
{
	public static class Unsafe
	{
		public static unsafe void FillBytes_Int(int value, byte[] buffer, int index = 0)
		{
			byte[] bytes = buffer;
			fixed (byte* b = &bytes[index])
				*((int*)b) = value;
			return;
		}

		public static int GetPower2(int value)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value++;
			return value;
		}
	}
}
