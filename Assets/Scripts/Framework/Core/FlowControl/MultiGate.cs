using System;
using System.Collections.Generic;
using Framework.Library.Singleton;

namespace Framework.Core.FlowControl
{

	public class MultiGate : AFlowControl
	{
		protected override void EnterFlow()
		{
			if (actionList != null && actionList.Length > 0)
			{
				if (0 <= ActiveIndex && ActiveIndex < actionList.Length && actionList[ActiveIndex] != null )
				{
					actionList[ActiveIndex]();
					ToNextActionIndex();
				}
			}
		}


		public MultiGate(): this(false, false, -1)
		{

		}
		public MultiGate(bool loop, bool randomRoute) : this (loop, randomRoute, -1)
		{

		}

		public MultiGate(bool loop, bool randomRoute, int startIndex)
		{
			IsLoop = loop;
			IsRandomRoute = randomRoute;
			ActiveIndex = startIndex;
		}

		public bool IsLoop { get; private set; }
		public bool IsRandomRoute { get; private set; }
		public int ActiveIndex { get; private set; }

		Action[] actionList;

		public void SetActions(Action[] actionList)
		{
			this.actionList = actionList;
			if (actionList != null)
			{
				if(ActiveIndex < 0 || ActiveIndex > actionList.Length)
				{
					ActiveIndex = 0;
				}
			}
			
		}

		readonly Random random = new Random();

		private void ToNextActionIndex()
		{
			if (this.actionList == null || actionList.Length == 0)
			{
				ActiveIndex = -1;
			}
			else
			{
				if (IsRandomRoute)
				{
					ActiveIndex = random.Next(0, actionList.Length);
				}
				else
				{
					ActiveIndex += 1;
				}
			}
		}
	}

	
}
