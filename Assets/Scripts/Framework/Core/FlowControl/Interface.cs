

using System;

namespace Framework.Core.FlowControl
{
	public interface IFlowControl
	{
		event Action OnExecute;
		void Execute();
	}

	public interface ITickable
	{
		bool TickEnabled { get; }
		void Tick();
	}
}
