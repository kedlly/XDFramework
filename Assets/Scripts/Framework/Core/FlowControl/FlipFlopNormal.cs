using System;
using System.Collections.Generic;
using Framework.Library.Singleton;

namespace Framework.Core.FlowControl
{
	public class FlipFlopNormal : AFlowControl
	{

		private Action[] callList;

		public int TotalExitCount { get; private set; }
		public int ActionIndex { get; private set; }

		public FlipFlopNormal(int exitCount)
		{
			if(exitCount < 2)
			{
				throw new Exception("use another way . ");
			}
			callList = new Action[exitCount];
			ActionIndex = 0;
			TotalExitCount = exitCount;
		}

		public void SetAction(int index, Action action)
		{
			if(0 <= index && index < TotalExitCount)
			{
				if(callList[index] == null)
				{
					callList[index] = action;
				}
				else
				{
					callList[index] += action;
				}
			}
			else
			{

			}
		}

		void IndexToNext()
		{
			ActionIndex++;
			if(ActionIndex >= TotalExitCount)
			{
				ActionIndex = 0;
			}
		}

		protected override void EnterFlow()
		{
			Action item = callList[ActionIndex];
			if(item != null)
			{
				item();
			}
			IndexToNext();
		}
	}

}
