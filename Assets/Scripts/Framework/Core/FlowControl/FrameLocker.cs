using System;
using UnityEngine;

namespace Framework.Core.FlowControl
{
	public class FrameLocker
	{
		float _frameRate = 60;
		public float frameRate
		{
			get
			{
				return _frameRate;
			}
			set
			{
				_frameRate = value;
				if (_frameRate == 0)
				{
					_frameRate = 1.0e5f ;
				}
				frameTime = 1 / _frameRate;
			}
		}

		public float frameTime { get; private set; }
		Action<float> frameAction;
		public FrameLocker(float frameRate, Action<float> updateFunc)
		{
			this.frameRate = frameRate;
			frameAction = updateFunc;
		}

		float lastTime = 0;
		

		public void Reset()
		{
			lastTime = Time.time;
		}

		public void Update()
		{
			var mDeltaTime = Time.time - lastTime;
			if (mDeltaTime > frameTime)
			{
				while (mDeltaTime >= frameTime)
				{
					if (frameAction != null)
					{
						frameAction(frameTime);
					}
					mDeltaTime -= frameTime;
				}
				lastTime = Time.time;
			}
			
		}
	}
}
