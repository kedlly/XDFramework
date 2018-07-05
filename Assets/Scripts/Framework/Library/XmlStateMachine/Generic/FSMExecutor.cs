
using System;

namespace Framework.Library.XMLStateMachine
{
	internal class FSMExecutor<T> where T : class
	{
		public string Name { get; private set; }
		internal State<T>[] states { get; private set; }
		internal State<T> currentState { get; private set; }
		public WeakReference Owner { get; private set; }
		public string CurrentStateName
		{
			get
			{
				if(currentState == null)
				{
					return "";
				}
				return currentState.FullName;
			}
		}

		internal FSMExecutor(StateMap<T> stateMap)
		{
			if(stateMap != null)
			{
				currentState = stateMap.Entry;
				states = stateMap.StateSet;
			}
		}

		public void SetTargetObject(T owner)
		{
			if (owner != null)
			{
				Owner = new WeakReference( owner);
				if (Owner.IsAlive && Owner.Target != null)
				{
					currentState.onEnter(Owner.Target as T);
				}
			}
		}

		public void Update()
		{
			if(Owner.IsAlive && Owner.Target != null)
			{
				currentState.onUpdate(Owner.Target as T);
			}
		}

		private TransitionRule<T> GetFiredTransition(string firedEvent)
		{
			return currentState.GetFiredTransition(Owner.Target as T, firedEvent);
		}

		public void PushEvent(string firedEvent)
		{
			var targetTrans = GetFiredTransition(firedEvent);
			var OwnerObject = Owner.Target as T;
			if (OwnerObject == null)
			{
				return;
			}
			if (targetTrans != null)
			{
				currentState = State<T>.GotoNext(OwnerObject, currentState, targetTrans.Target);
			}
		}

		/*
		public static FSMExecutor<T> LoadFromXML(string xml)
		{
			return new FSMExecutor<T>(FSMExecutorFactory<T>.LoadFromXML(xml));
		}
		*/

	}

}
