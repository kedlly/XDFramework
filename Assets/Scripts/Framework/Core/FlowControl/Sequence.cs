using System;
using System.Collections.Generic;
using Framework.Library.Singleton;

namespace Framework.Core.FlowControl
{

	public class Sequence : AFlowControl
	{
		protected override void EnterFlow()
		{
			if (OnAction != null)
			{
				OnAction();
			}
		}

		public Sequence()
		{

		}

		public Sequence(Action[] actions) : this()
		{
			if (actions != null)
			{
				foreach (var action in actions)
				{
					this.Then(action);
				}
			}
		}

		private event Action OnAction;
		
		public Sequence Then(Action action)
		{
			OnAction += action;
			return this;
		}
	}


}
