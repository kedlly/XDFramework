using System;
using System.Collections.Generic;
using Framework.Library.Singleton;

namespace Framework.Core.FlowControl
{

	public class Gate : AFlowControl
	{
		private bool isClosed = false;
		protected override void EnterFlow()
		{
			if(IsOpen)
			{
				if(OnExit != null)
				{
					OnExit();
				}
			}
		}

		public Gate() : this(false) { }

		public Gate(bool startClosed)
		{
			isClosed = startClosed;
		}

		public bool IsOpen { get { return !isClosed; } }
		public bool IsClosed { get { return isClosed; } }

		public event Action OnExit;

		public void Open()
		{
			isClosed = false;
		}

		public void Close()
		{
			isClosed = true;
		}

		public void Toggle()
		{
			isClosed = !isClosed;
		}
	}
}
