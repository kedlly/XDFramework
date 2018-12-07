using UnityEngine;
using System.Collections.Generic;
using System;
using Framework.Library.Singleton;
using System.Linq;

namespace Framework.Core
{
	public class SystemEvent : EventTypeIndexImpl<SystemEvent>
	{
		private SystemEvent() : base() { }
	}
	public sealed class SystemEventManager : ToSingleton<SystemEventManager>, IEventManager<SystemEvent, EventArgs>
	{

		private InternalMgr internalManager = new InternalMgr();
		private class InternalMgr : EventManagerBase<SystemEvent, EventArgs>
		{
			
		}
		private SystemEventManager()	{ }

		protected override void OnSingletonInit(){}

		public void Release()
		{
			internalManager.Release();
		}

		public void AddListener(SystemEvent newEvent, EventHandler<EventArgs> newListener)
		{
			internalManager.AddListener(newEvent, newListener);
		}

		public void RemoveListener(SystemEvent existEvent, EventHandler<EventArgs> listener)
		{
			internalManager.RemoveListener(existEvent, listener);
		}

		public void SendEvent(SystemEvent theEvent, object sender, EventArgs eventArgs)
		{
			internalManager.SendEvent(theEvent, sender, eventArgs);
		}

		public void RemoveListener(SystemEvent theEvent)
		{
			internalManager.RemoveListener(theEvent);
		}

		public void RemoveObjectListener(object obj)
		{
			internalManager.RemoveObjectListener(obj);
		}

		public void RemoveListenerFreedom(SystemEvent[] keys, Func<EventHandler<EventArgs>, bool> predicate)
		{
			internalManager.RemoveListenerFreedom(keys, predicate);
		}

		public void RemoveObjectListener(object obj, SystemEvent theEvent)
		{
			internalManager.RemoveObjectListener(obj, theEvent);
		}

		public void RemoveTypeStaticListener(Type targetType)
		{
			internalManager.RemoveTypeStaticListener(targetType);
		}

		public void RemoveTypeStaticListener(Type targetType, SystemEvent theEvent)
		{
			internalManager.RemoveTypeStaticListener(targetType, theEvent);
		}

		public void RemoveTypeAllListener(Type targetType)
		{
			internalManager.RemoveTypeAllListener(targetType);
		}

		public void RemoveTypeAllListener(Type targetType, SystemEvent theEvent)
		{
			internalManager.RemoveTypeAllListener(targetType, theEvent);
		}

		public int Count(SystemEvent @event = null)
		{
			return internalManager.Count(@event);
		}

		public void Update()
		{
			internalManager.Update();
		}

		public bool IsExistEventHandle(EventHandler<EventArgs> handler, SystemEvent @event = null)
		{
			return internalManager.IsExistEventHandle(handler, @event);
		}
	}

	
}