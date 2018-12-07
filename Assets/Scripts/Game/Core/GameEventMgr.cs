using Framework;
using Framework.Core;
using Framework.Library.ObjectPool;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;
using System;

namespace Game.Core
{
	[Serializable]
	public abstract class GameEventArgs : EventArgs
	{
		public readonly GameEventType EventType = GameEventType.kUnknow;
		public bool IsUsed { get; private set; }
		public void Use() { IsUsed = true; }
		public void Reset() { IsUsed = false; }

		public bool AutoReset { get; private set; }

		public GameEventArgs(GameEventType eventType = null, bool autoReset = false)
		{
			if (eventType != null)
			{
				this.EventType = eventType;
			}
			AutoReset = autoReset;
		}

		public static T GetPublicStaticPredefinition<T>(string name) where T : GameEventArgs
		{
			return typeof(T).GetPublicStaticField<T>(name);
		}

		protected static T Allocate<T>() where T : GameEventArgs
		{
			return ObjectCache.GlobalCache.Allocate<T>();
		}

		protected static void Recycle<T>(T target) where T : GameEventArgs
		{
			ObjectCache.GlobalCache.Recycle(target);
		}

		protected virtual void OnInvokedDone() { }

		public void InvokedDone()
		{
			OnInvokedDone();
		}
	}

	public partial class GameEventType : EventTypeIndexImpl<GameEventType>
	{
		private GameEventType() : base() { }
		public static readonly GameEventType kUnknow			= new GameEventType();
		public static readonly GameEventType kInput_Keyboard	= new GameEventType();
		public static readonly GameEventType kInput_Mouse		= new GameEventType();
		public static readonly GameEventType kUICommand			= new GameEventType();
		//add other hardware or control event here

		//add system inside event here
		public static readonly GameEventType kGameEvent_Flag	= new GameEventType();        // Game Event start point , don't use it directly
		public static readonly GameEventType kOther				= new GameEventType();
	}



	public class GameEventManager : ToSingleton<GameEventManager>, IEventManager<GameEventType, GameEventArgs>, IManageredObject
	{
		private class BreakableEventManager : EventManagerBase<GameEventType, GameEventArgs>
		{
			protected override bool BreakInvocationList(GameEventArgs args)
			{
				bool breakOp = args != null && args.IsUsed;
				if (breakOp && args.AutoReset)
				{
					args.Reset();
				}
				return breakOp;
			}
			protected override void InvokedDone(GameEventArgs args)
			{
				if (args != null)
				{
					args.InvokedDone();
				}
			}
		}
		protected override void OnSingletonInit()
		{
			var ms = FrameworkUtils.GamePlay.GetSubManager<CoreEventSystem>();
			if (ms != null)
			{
				ms.Register(this);
			}
		}

		public void Release()
		{
			implEventMgr.Release();
		}

		public void AddListener(GameEventType newEvent, EventHandler<GameEventArgs> newListener)
		{
			implEventMgr.AddListener(newEvent, newListener);
		}

		public void RemoveListener(GameEventType existEvent, EventHandler<GameEventArgs> listener)
		{
			implEventMgr.RemoveListener(existEvent, listener);
		}

		public void SendEvent(GameEventType theEvent, object sender, GameEventArgs eventArgs)
		{
			implEventMgr.SendEvent(theEvent, sender, eventArgs);
		}

		public void RemoveListener(GameEventType theEvent)
		{
			implEventMgr.RemoveListener(theEvent);
		}

		public void RemoveObjectListener(object obj)
		{
			implEventMgr.RemoveObjectListener(obj);
		}

		public void RemoveListenerFreedom(GameEventType[] keys, Func<EventHandler<GameEventArgs>, bool> predicate)
		{
			implEventMgr.RemoveListenerFreedom(keys, predicate);
		}

		public void RemoveObjectListener(object obj, GameEventType theEvent)
		{
			implEventMgr.RemoveObjectListener(obj, theEvent);
		}

		public void RemoveTypeStaticListener(Type targetType)
		{
			implEventMgr.RemoveTypeStaticListener(targetType);
		}

		public void RemoveTypeStaticListener(Type targetType, GameEventType theEvent)
		{
			implEventMgr.RemoveTypeStaticListener(targetType, theEvent);
		}

		public void RemoveTypeAllListener(Type targetType)
		{
			implEventMgr.RemoveTypeAllListener(targetType);
		}

		public void RemoveTypeAllListener(Type targetType, GameEventType theEvent)
		{
			implEventMgr.RemoveTypeAllListener(targetType, theEvent);
		}

		public int Count(GameEventType theEvent = null)
		{
			return implEventMgr.Count(theEvent);
		}

		public void Update()
		{
			implEventMgr.Update();
		}

		public bool IsExistEventHandle(EventHandler<GameEventArgs> handler, GameEventType @event = null)
		{
			return implEventMgr.IsExistEventHandle(handler, @event);
		}

		public void Tick()
		{
			this.Update();
		}

		private GameEventManager() { }

		private BreakableEventManager implEventMgr = new BreakableEventManager();

		public bool TickEnabled
		{
			get
			{
				return true;
			}
		}

		public bool IsActiving { get { return true; } }
	}
}
