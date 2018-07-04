using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core.FlowControl
{
	public abstract class AFlowControl : IFlowControl
	{
		public event Action OnExecute;

		public void Execute()
		{
			if(OnExecute != null)
			{
				OnExecute();
			}
			EnterFlow();
		}

		protected abstract void EnterFlow();

	}

	public abstract class ATickable : AFlowControl, ITickable
	{
		private bool Tickable = false;
		public bool TickEnabled
		{
			get { return Tickable; }
		}

		public enum TimeDirection
		{
			Forward = 1,
			Stopped = 0,
			Backword = -1
		}

		public TimeDirection Direction { get; protected set; }

		private float playRate = 1.0f;
		public float PlayRate { get { return playRate; } set { playRate = value; } }

		public bool IsPlaying { get { return TickEnabled && Direction != TimeDirection.Stopped; } }

		protected void EnableTick()
		{
			if(!Tickable)
			{
				Tickable = true;
				FlowControlMananger.Instance.Register(this);
			}
		}

		protected void DisableTick()
		{
			if(Tickable)
			{
				Tickable = false;
				FlowControlMananger.Instance.UnRegister(this);
			}
		}

		public void Tick()
		{
			if(Tickable)
			{
				float deltaTime = UnityEngine.Time.deltaTime;
				deltaTime *= (int)Direction * PlayRate;
				OnTick(deltaTime);
			}
		}

		protected virtual void OnTick(float deltaTime) { }
	}

}
