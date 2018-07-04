using System;
using System.Collections.Generic;

namespace Framework.Core.FlowControl
{
	public class Timeline : ATickable
	{

		protected override void EnterFlow()
		{

		}
	
		public bool IsLoop { get; set; }

		public bool IsInTimelineStartPoint
		{
			get
			{
				return CurrenTime == 0;
			}
		}

		public bool IsInTimelineEndPoint
		{
			get
			{
				return CurrenTime == TimeLength;
			}
		}

		public void Play()
		{
			if (IsInTimelineStartPoint || IsInTimelineEndPoint)
			{
				PlayFromStart();
			}
			else
			{
				Direction = TimeDirection.Forward;
				EnableTick();
			}
		}
		public void PlayFromStart()
		{
			CurrenTime = 0.0f;
			IndexEvent = 0;
			Direction = TimeDirection.Forward;
			EnableTick();
		}
		public void Stop()
		{
			Direction = TimeDirection.Stopped;
			DisableTick();
		}
		public void Reverse()
		{
			if(Direction != TimeDirection.Backword)
			{
				Direction = TimeDirection.Backword;
				if(CurrenTime != timeDestination)
				{
					EnableTick();
				}
			}
		}
		public void ReverseFromEnd()
		{
			CurrenTime = TimeLength;
			IndexEvent = timeLineEvents.Count - 1;
			Direction = TimeDirection.Backword;
			EnableTick();
		}
		public void SetNewTime(float time, bool fireEvent, bool fireUpdate)
		{
			CurrenTime = time;
			if (fireEvent)
			{
				CheckAndFireTimeEvent(CurrenTime);
			}
			if (fireUpdate && OnUpdate != null)
			{
				OnUpdate();
			}
		}

		public void SetTimelineLength(float newLength)
		{
			TimeLength = newLength;
		}

		

		public Timeline(float timelineLength) : this(timelineLength, false) { }

		public Timeline(float timelineLength, bool autoPlay)
		{
			Direction = TimeDirection.Stopped;
			TimeLength = timelineLength;
			CurrenTime = 0.0f;
			PlayRate = 1.0f;
			IsLoop = false;
			if(autoPlay)
			{
				Play();
			}
		}

		protected override void OnTick(float timeDelta)
		{
			if(CurrenTime != timeDestination)
			{
				CurrenTime += timeDelta;
				if(OnUpdate != null)
				{
					OnUpdate();
				}
				CheckAndFireTimeEvent(CurrenTime);
			}
			else
			{
				if(!IsLoop)
				{
					Stop();
					if(OnFinished != null)
					{
						OnFinished();
					}
				}
				else
				{
					switch(Direction)
					{
						case TimeDirection.Backword:
							ReverseFromEnd();
							break;
						case TimeDirection.Forward:
							PlayFromStart();
							break;
						default:
							break;
					}
				}
			}
		}

		private float timeNonius = 0.0f;
		public float CurrenTime
		{
			get
			{
				return timeNonius;
			}
			private set
			{
				timeNonius = ClampTime(value);
			}
		}

		private float ClampTime(float value)
		{
			return value < 0f ? 0f : value > TimeLength ? TimeLength : value;
		}

		float timeDestination
		{
			get
			{
				return Direction == TimeDirection.Forward ?
							TimeLength :
						Direction == TimeDirection.Backword ?
							0.0f :
							CurrenTime;
			}
		}
		public float TimeLength { get; private set; }

		public event Action OnUpdate;
		public event Action OnFinished;

		public void AddEvent(float time, Action action)
		{
			TimeLineEvent newEvent = new TimeLineEvent() { timeStart = time, eventAction = action };
			timeLineEvents.Add(newEvent);
			timeLineEvents.Sort();
		}

		private int IndexEvent = 0;
		public void CheckAndFireTimeEvent(float time)
		{
			if (IndexEvent < 0 || IndexEvent > timeLineEvents.Count-1 || Direction == TimeDirection.Stopped)
			{
				return;
			}
			var test = timeLineEvents[IndexEvent];
			if(Direction == TimeDirection.Forward)
			{
				if(time >= test.timeStart)
				{
					if(test.eventAction != null)
					{
						test.eventAction();
					}
					IndexEvent++;
					CheckAndFireTimeEvent(time);
				}
			}
			else if(Direction == TimeDirection.Backword)
			{
				if(time <= test.timeStart)
				{
					if(test.eventAction != null)
					{
						test.eventAction();
					}
					IndexEvent --;
					CheckAndFireTimeEvent(time);
				}
			}
			else
			{

			}
		}

		struct TimeLineEvent : IComparable<TimeLineEvent>
		{
			public float timeStart;
			public Action eventAction;

			int IComparable<TimeLineEvent>.CompareTo(TimeLineEvent other)
			{
				float distance = timeStart - other.timeStart;
				return distance < 0 ? -1 : distance > 0 ? 1 : 0;
			}
		}

		List<TimeLineEvent> timeLineEvents = new List<TimeLineEvent>();
	}
}
