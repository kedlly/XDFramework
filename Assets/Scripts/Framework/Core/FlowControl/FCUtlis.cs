using System;
using System.Collections.Generic;
using Framework.Library.Singleton;

namespace Framework.Core.FlowControl
{

	internal class FlowControlMananger : ToSingleton<FlowControlMananger>
	{
		private FlowControlMananger() {}

		protected override void OnSingletonInit()
		{
			
		}

		HashSet<ITickable> registerList = new HashSet<ITickable>();
		HashSet<ITickable> unregisterList = new HashSet<ITickable>();

		public void Register(ITickable fc)
		{
			//if (!registerList.Contains(fc))
			{
				registerList.Add(fc);
			}
		}

		public void UnRegister(ITickable fc)
		{
			//if (!unregisterList.Contains(fc) && !registerList.Contains(fc))
			{
				unregisterList.Add(fc);
			}
		}

		public void Tick()
		{
			foreach (var fc in registerList)
			{
				if(fc.TickEnabled)
				{
					fc.Tick();
				}
				else
				{
					UnRegister(fc);
				}
			}
			registerList.ExceptWith(unregisterList);
			/*
			foreach (var fc in unregisterList)
			{
				registerList.Remove(fc);
			}*/
			unregisterList.Clear();
		}
	}
}
