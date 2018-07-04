

using System;

namespace Framework.Core.FlowControl
{
	public class DoN : AFlowControl
	{

		public int DefalutTimes { get; private set; }

		protected int timeCount;
		public event Action OnExit;

		public DoN(int defalutTimes)
		{
			DefalutTimes = defalutTimes;
			timeCount = defalutTimes;
		}

		public void Reset()
		{
			timeCount = DefalutTimes;
		}

		public int Counter { get { return DefalutTimes - timeCount; } }

		protected override void EnterFlow()
		{
			if(timeCount > 0)
			{
				timeCount--;
				if(OnExit != null)
				{
					OnExit();
				}
			}
		}
	}
}
