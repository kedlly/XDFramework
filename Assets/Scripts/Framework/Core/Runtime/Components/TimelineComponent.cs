using Framework.Core.FlowControl;
using UnityEngine;
using System;
using Framework.Core.Attributes;


namespace Framework.Core.Runtime
{
	[Serializable]
	public struct TimelineEvent
	{
		public float time;
		public string eventName;
	}

	public class TimelineComponent : MonoBehaviour
	{
		public Timeline TimelineObject = new Timeline(1);

		[Header("Timeline Settings")]
		public float length = 10;
		public float startTime = 0.0f;
		public float playRate = 1.0f;
		public bool loop = false;
		public bool autoPlay = true;
		[ShowOnly]
		public bool isPlaying;
		//public bool ReverseFromEnd
		
		[Header("Timeline Events")]
		public TimelineEvent[] eventList;

		private void Awake()
		{
			TimelineObject.SetTimelineLength(length);
			TimelineObject.SetNewTime(startTime, false, false);
			TimelineObject.IsLoop = loop;
			TimelineObject.PlayRate = playRate;

			if(eventList != null && eventList.Length > 0)
			{
				foreach(var e in eventList)
				{
					TimelineObject.AddEvent(e.time, () => SendMessage(e.eventName));
				}
			}
		}

		// Use this for initialization
		void Start()
		{
			if(autoPlay)
			{
				TimelineObject.Play();
			}
		}

		private void OnDestroy()
		{
			if (TimelineObject.IsPlaying)
			{
				TimelineObject.Stop();
			}
			TimelineObject = null;
		}

#if UNITY_EDITOR
		// Update is called once per frame
		void Update()
		{
			isPlaying = TimelineObject.IsPlaying;
		}
#endif
	}
}