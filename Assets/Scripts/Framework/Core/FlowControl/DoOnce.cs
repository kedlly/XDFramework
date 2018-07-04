using System;


namespace Framework.Core.FlowControl
{
	public class DoOnce : DoN
	{
		public DoOnce(bool startClosed) : base(1)
		{
			if(startClosed)
			{
				timeCount = 0;
			}
		}

		public DoOnce() : this(false) { }
	}
}