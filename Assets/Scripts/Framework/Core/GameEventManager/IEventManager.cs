using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Framework.Utils.Extensions;

namespace Framework.Core
{

	public abstract class EventTypeIndexImpl<T> where T : EventTypeIndexImpl<T>
	{
		public static int CreationIndex { get;private set; }
		public int TypeIndex { get; private set; }
		public string Name { get { return GetKeyName(); } }
		public override int GetHashCode()
		{
			return TypeIndex;
		}
		protected EventTypeIndexImpl()
		{
			TypeIndex = CreationIndex ++;
		}

		protected EventTypeIndexImpl(int manualIndex)
		{
			TypeIndex = manualIndex;
		}

		public string GetKeyName()
		{
			return GetEventTypeName(TypeIndex);
		}
		public override string ToString()
		{
			return base.ToString() + "." + Name;
		}


		public static string GetEventTypeName(int typeIndex)
		{
			return GetFieldInfoByIndex(typeIndex).Name;
		}

		private static FieldInfo GetFieldInfoByIndex(int typeIndex)
		{
			var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
			return fields.Where(it => (it.GetValue(null) as T).TypeIndex == typeIndex).Single();
		}

		public static T GetEventTypeByIndex(int index)
		{
			return GetFieldInfoByIndex(index).GetValue(null) as T;
		}

		public static T GetEventTypeByName(string name)
		{
			return typeof(T).GetPublicStaticField<T>(name);
		}
	}

	public class EventTypeEquality<TEventType> : IEqualityComparer<TEventType> where TEventType : EventTypeIndexImpl<TEventType>
	{

		public static readonly EventTypeEquality<TEventType> comparer = new EventTypeEquality<TEventType>();

		public bool Equals(TEventType x, TEventType y)
		{
			return x.TypeIndex == y.TypeIndex;
		}

		public int GetHashCode(TEventType obj)
		{
			return obj.TypeIndex;
		}
	}

	public interface IEventManager<TEvent, TEventArgs>// : IManageredObject
		where TEventArgs : EventArgs
		where TEvent : EventTypeIndexImpl<TEvent>
	{
		/// <summary>
		/// 清空所有事件响应
		/// </summary>
		void Release();

		/// <summary>
		/// 添加特定事件类型的监听器
		/// </summary>
		/// <param name="newEvent"></param>
		/// <param name="newListener"></param>
		void AddListener(TEvent newEvent, EventHandler<TEventArgs> newListener);

		/// <summary>
		/// 移除 特定事件的监听
		/// </summary>
		/// <param name="existEvent"></param>
		/// <param name="listener"></param>
		void RemoveListener(TEvent existEvent, EventHandler<TEventArgs> listener);
		/// <summary>
		/// 激发事件
		/// </summary>
		/// <param name="theEvent"></param>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		void SendEvent(TEvent theEvent, object sender, TEventArgs eventArgs);
		/// <summary>
		/// 移除特定事件的所有监听
		/// </summary>
		/// <param name="theEvent"></param>
		void RemoveListener(TEvent theEvent);
		/// <summary>
		/// 移除与特定对象相关的所有事件监听
		/// </summary>
		/// <param name="obj"></param>
		void RemoveObjectListener(object obj);
		/// <summary>
		/// 当keyss不为空且 keys.length > 0 且 predicate 不为null
		///		移除 keys 所指定的 并满足 predicate 条件的 监听
		/// 当keys为空或者 keys.length == 0 且 predicate 不为null
		///		移除 事件系统中所有 满足 predicate 的监听
		///	当keys不为空且 keys.length > 0 且 predicate 为 null
		///		移除 事件系统中所有 keys 对应的所有监听
		///	当keys为空或者 keys.length == 0 时 且 predicate 为null
		///		无操作
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="predicate"></param>
		void RemoveListenerFreedom(TEvent[] keys, Func<EventHandler<TEventArgs>, bool> predicate);

		/// <summary>
		/// 移除与特定监听对象相关的特定事件响应
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="theEvent"></param>
		void RemoveObjectListener(object obj, TEvent theEvent);

		/// <summary>
		/// 移除特定类型上的所有响应 (listener 为该类型上的某静态函数)
		/// </summary>
		/// <param name="targetType"></param>
		void RemoveTypeStaticListener(Type targetType);
		/// <summary>
		/// 移除特定类型上的特定事件类型响应 (listener 为该类型上的某静态函数)
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="theEvent"></param>
		void RemoveTypeStaticListener(Type targetType, TEvent theEvent);
		/// <summary>
		/// 移除特定类型上的静态&实例 响应
		/// </summary>
		/// <param name="targetType"></param>
		void RemoveTypeAllListener(Type targetType);
		/// <summary>
		/// 移除特定类型上的静态&实例 的特定事件类型 响应
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="theEvent"></param>
		void RemoveTypeAllListener(Type targetType, TEvent theEvent);
		/// <summary>
		/// 获取特定类型事件的listener数量
		/// theEvent == null, 返回所有listener 数量
		/// </summary>
		/// <param name="theEvent"></param>
		/// <returns></returns>
		int Count(TEvent theEvent = null);
		/// <summary>
		/// 轮询
		/// </summary>
		void Update();
		/// <summary>
		/// 检测是否存在特定的事件响应
		/// event 为null 时，表示 所有事件
		/// </summary>
		/// <param name="event"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		bool IsExistEventHandle(EventHandler<TEventArgs> handler, TEvent @event = null);

	}

	public abstract class EventManagerBase<TEvent, TEventArgs> : IEventManager<TEvent, TEventArgs> 
		where TEventArgs : EventArgs
		where TEvent : EventTypeIndexImpl<TEvent>
	{
		private readonly Dictionary<TEvent, EventHandler<TEventArgs>> dictEventCallBack = 
								new Dictionary<TEvent, EventHandler<TEventArgs>>(16, EventTypeEquality<TEvent>.comparer);
		protected readonly object m_syncRoot = new object();

		public void Release() { dictEventCallBack.Clear(); }

		public void AddListener(TEvent newEvent, EventHandler<TEventArgs> newListener)
		{
			if (newListener == null) return;
			EventHandler<TEventArgs> listeners = null;
			lock (m_syncRoot)
			{
				if (dictEventCallBack.TryGetValue(newEvent, out listeners))
				{
					dictEventCallBack[newEvent] = listeners + newListener;
				}
				else
				{
					dictEventCallBack.Add(newEvent, newListener);
				}
			}
		}

		public void RemoveListener(TEvent existEvent, EventHandler<TEventArgs> listener)
		{
			if (listener == null || !dictEventCallBack.ContainsKey(existEvent)) return;
			EventHandler<TEventArgs> listeners = null;
			lock (m_syncRoot)
			{
				if (dictEventCallBack.TryGetValue(existEvent, out listeners))
				{
					dictEventCallBack[existEvent] = listeners - listener;
					if (dictEventCallBack[existEvent] == null)
					{
						dictEventCallBack.Remove(existEvent);
					}
				}
				else
				{
					UnityEngine.Debug.LogWarning("RemoveListener:the dictionary don't contain CallBack Event:" + existEvent);
				}
			}
		}


		private class BackgroundThreadEventWrapper
		{
			public TEvent Event;
			public object Sender;
			public TEventArgs EventArgs;
		}
		private Queue<BackgroundThreadEventWrapper> backgroundEvents = new Queue<BackgroundThreadEventWrapper>();

		public void SendEvent(TEvent theEvent, object sender, TEventArgs eventArgs)
		{
			EventHandler<TEventArgs> listeners = null;
			lock (m_syncRoot)
			{
				if (System.Threading.Thread.CurrentThread.ManagedThreadId != FrameworkUtils.Application.MainThreadId)
				{
					backgroundEvents.Enqueue(new BackgroundThreadEventWrapper() { Event = theEvent, Sender = sender, EventArgs = eventArgs });
					return;
				}
				if (dictEventCallBack.TryGetValue(theEvent, out listeners) && listeners != null)
				{
					var itor = listeners.GetInvocationList().OfType<EventHandler<TEventArgs>>();
					foreach (var listener in itor)
					{
						listener(sender, eventArgs);
						if (BreakInvocationList(eventArgs))
						{
							break;
						}
					}
					InvokedDone(eventArgs);
				}
				else
				{
					UnityEngine.Debug.LogWarning("SendEvent:the dictionary don't contain CallBack Event:" + theEvent);
				}
			}
		}

		protected virtual bool BreakInvocationList(TEventArgs args)
		{
			return false;
		}

		protected virtual void InvokedDone(TEventArgs args)
		{
		}

		public void RemoveListener(TEvent theEvent)
		{
			lock (m_syncRoot)
			{
				if (dictEventCallBack.ContainsKey(theEvent))
				{
					dictEventCallBack.Remove(theEvent);
				}
			}
		}

		public void RemoveObjectListener(object obj)
		{
			lock (m_syncRoot)
			{
				RemoveFunc(dictEventCallBack.Keys.ToArray(), it => it.Target == obj);
			}
		}

		public void RemoveListenerFreedom(TEvent[] keys, Func<EventHandler<TEventArgs>, bool> predicate)
		{
			if ((keys == null || keys.Length == 0) && predicate == null)
			{
				return;
			}
			lock (m_syncRoot)
			{
				if (keys == null || keys.Length == 0)
				{
					keys = dictEventCallBack.Keys.ToArray();
				}
				if (predicate == null)
				{
					predicate = it => true;
				}
				RemoveFunc(keys, predicate);
			}
		}

		private void RemoveFunc(TEvent[] keys, Func<EventHandler<TEventArgs>, bool> predicate)
		{
			foreach (var key in keys)
			{
				var listeners = dictEventCallBack[key];
				if (listeners != null)
				{
					var ils = listeners.GetInvocationList().OfType<EventHandler<TEventArgs>>().Where(predicate).ToArray();
					if (ils.Count() > 0)
					{
						foreach (var i in ils)
						{
							dictEventCallBack[key] = dictEventCallBack[key] - i;
						}
						if (dictEventCallBack[key] == null)
						{
							dictEventCallBack.Remove(key);
						}
					}
				}
			}
		}

		public void RemoveObjectListener(object obj, TEvent theEvent)
		{
			lock (m_syncRoot)
			{
				if (dictEventCallBack.ContainsKey(theEvent))
				{
					RemoveFunc(new TEvent[] { theEvent }, it => it.Target == obj);
				}
			}
		}

		public void RemoveTypeStaticListener(Type targetType)
		{
			lock (m_syncRoot)
			{
				RemoveFunc(dictEventCallBack.Keys.ToArray()
					, it => it.Target == null 
						&& (it.Method.DeclaringType == targetType || it.Method.ReflectedType.DeclaringType == targetType));
			}
		}

		public void RemoveTypeStaticListener(Type targetType, TEvent theEvent)
		{
			lock (m_syncRoot)
			{
				if (dictEventCallBack.ContainsKey(theEvent))
				{
					RemoveFunc(new TEvent[] { theEvent }
						, it => it.Target == null 
							&& (it.Method.DeclaringType == targetType || it.Method.ReflectedType.DeclaringType == targetType));
				}
			}
		}

		public void RemoveTypeAllListener(Type targetType)
		{
			lock (m_syncRoot)
			{
				RemoveFunc(dictEventCallBack.Keys.ToArray()
					, it => it.Method.DeclaringType == targetType || it.Method.ReflectedType.DeclaringType == targetType);
			}
		}

		public void RemoveTypeAllListener(Type targetType, TEvent theEvent)
		{
			lock (m_syncRoot)
			{
				if (dictEventCallBack.ContainsKey(theEvent))
				{
					RemoveFunc(new TEvent[] { theEvent }
						, it => it.Method.DeclaringType == targetType || it.Method.ReflectedType.DeclaringType == targetType);
				}
			}
		}

		public int Count(TEvent theEvent = null)
		{
			lock (m_syncRoot)
			{
				return dictEventCallBack.Where(it =>
				{
					return (theEvent == null || it.Key == theEvent) && it.Value != null;
				}).SelectMany(it => it.Value.GetInvocationList()).Count();
			}
		}

		public void Update()
		{
			lock (m_syncRoot)
			{
				UnityEngine.Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == FrameworkUtils.Application.MainThreadId);
				while (backgroundEvents.Count > 0)
				{
					var @event = backgroundEvents.Dequeue();
					this.SendEvent(@event.Event, @event.Sender, @event.EventArgs);
				}
			}
		}

		public bool IsExistEventHandle(EventHandler<TEventArgs> handler, TEvent @event = null)
		{
			return dictEventCallBack.Where(it => (@event == null || it.Key == @event) && it.Value != null)
				.SelectMany(it => it.Value.GetInvocationList())
				.Any(it => (it as EventHandler<TEventArgs>) == handler);
		}
	}
}