using System;

namespace Framework.Core.FlowControl
{
	public sealed class Delay : ATickable
	{
		protected override void EnterFlow()
		{
			Play();
		}

		public float DelayTime { get; private set; }

		public Delay(float time)
		{
			DelayTime = time;
			Reset();
		}

		private float timeNonius = 0.0f;
		private float CurrenTime
		{
			get
			{
				return timeNonius;
			}
			set
			{
				timeNonius = value < 0f ? 0f : value > DelayTime ? DelayTime : value;
			}
		}
		void Reset()
		{
			CurrenTime = 0.0f;
			Direction = TimeDirection.Stopped;
			DisableTick();
		}

		void Play()
		{
			Direction = TimeDirection.Forward;
			EnableTick();
		}

		protected override void OnTick(float deltaTime)
		{
			if(CurrenTime != DelayTime)
			{
				CurrenTime += deltaTime;
				if(CurrenTime == DelayTime)
				{
					FlowControlUtils.TryActivateAction(OnExit);
					Reset();
				}
			}
		}

		public event Action OnExit;
	}
}
