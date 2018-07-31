

using System;

namespace Framework.Core.FlowControl
{
	public interface IFlowControl
	{
		event Action OnExecute;
		void Execute();
	}

	public static class FlowControlUtils
	{
		public static void TryActivateAction(Action action)
		{
			try
			{
				if (action != null)
				{
					action();
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
			}
		}
	}
}
